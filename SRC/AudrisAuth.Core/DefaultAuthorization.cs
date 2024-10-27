using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;

using AudrisAuth.Exceptions;

using DynamicExpresso;

namespace AudrisAuth
{
    /// <summary>
    /// Default authorization class that implements the IAuthorization interface.
    /// Derive from this class to implement the authorization rules for a specific type.
    /// </summary>
    /// <typeparam name="T">Type to wich authorization is required</typeparam>
    public abstract class DefaultAuthorization<T> : IAuthorization<T>
    {
        /// <summary>
        /// Dictionary of available actions with their rules, accessible by name from derived classes.
        /// </summary>
        protected readonly Dictionary<string, AuthorizationAction> _availableActions = new Dictionary<string, AuthorizationAction>();

        /// <summary>
        /// Check if user can perform a generic action
        /// </summary>
        /// <param name="user">User that requires to perform the action.</param>
        /// <param name="action">Name of the action.</param>
        /// <returns>True if the user is allowed to perform the action.</returns>
        /// <exception cref="ArgumentNullException">If user is null</exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotRecognizedActionException"></exception>
        /// <exception cref="InstanceActionException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        // Implement the methods from IAuthorization<T>
        public bool Can(ClaimsPrincipal user, string action)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Action cannot be null or empty.", nameof(action));

            if (!_availableActions.TryGetValue(action, out var actionInfo))
                throw new NotRecognizedActionException(typeof(T), action);

            if (actionInfo.IsInstanceAction)
                throw new InstanceActionException(typeof(T), action);

            // Initialize user variables
            var interpreter = CreateInterpreter(user);

            // Compile and execute the rule
            try
            {
                var result = interpreter.Eval<bool>(actionInfo.Rule);
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error evaluating authorization rule.", ex);
            }
        }

        /// <summary>
        /// Check if user can perform a specific action on an instance of resource type T
        /// </summary>
        /// <param name="user">
        /// The user that requires to perform the action.
        /// </param>
        /// <param name="resource">
        /// The resource on which the action is to be performed.
        /// </param>
        /// <param name="action">
        /// The action to be performed.
        /// </param>
        /// <returns>
        /// True if the user is allowed to perform the action.
        /// </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotRecognizedActionException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Can(ClaimsPrincipal user, T resource, string action)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (string.IsNullOrWhiteSpace(action)) throw new ArgumentException("Action cannot be null or empty.", nameof(action));

            if (!_availableActions.TryGetValue(action, out var actionInfo))
                throw new NotRecognizedActionException(typeof(T), action);

            // Initialize user variables
            var interpreter = CreateInterpreter(user);

            // Set the resource variable
            interpreter.SetVariable("Resource", resource);

            // Compile and execute the rule
            try
            {
                var result = interpreter.Eval<bool>(actionInfo.Rule);
                return result;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error evaluating authorization rule.", ex);
            }
        }

        /// <summary>
        /// Get the list of available actions for the resource type T
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AuthorizationAction> GetAvailableActions()
        {
            return _availableActions.Values;
        }

        /// <summary>
        /// Creates and configures an interpreter for evaluating the authorization rules.
        /// </summary>
        /// <param name="user">The user for whom to create the interpreter.</param>
        /// <returns>An instance of <see cref="Interpreter"/> configured with user variables and functions.</returns>
        protected Interpreter CreateInterpreter(ClaimsPrincipal user)
        {

            // Extract user information
            var userId = GetUserId(user);
            var userRoles = GetUserRoles(user);
            var userClaims = GetClaims(user);

            // Set variables
            var interpreter = new Interpreter()
                .Reference(typeof(string))
                .Reference(typeof(int))
                .Reference(typeof(decimal))
                .Reference(typeof(long))
                .Reference(typeof(double))
                .Reference(typeof(DateTime))
                .Reference(typeof(bool))
                .SetVariable("UserId", userId)
                .SetVariable("UserRoles", userRoles)
                .SetVariable("UserClaims", userClaims);

            ConfigureInterpreter(interpreter);

            return interpreter;
        }

        /// <summary>
        /// Allow to configure the interpreter with custom variables
        /// in the derived class
        /// </summary>
        /// <param name="interpreter"></param>
        protected virtual void ConfigureInterpreter(Interpreter interpreter)
        {
            
        }

        /// <summary>
        /// Function used to extract the user id from the claims.
        /// Can be overridden in derived classes to extract the user id from a different claim.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The first value for a NameIdentifier claim</returns>
        protected virtual string GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Function used to extract the claims from the user.
        /// Can be overridden in derived classes to extract the claims in a different way.
        /// The claims are returned as a list of strings in the format "Type:Value".
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        protected virtual List<string> GetClaims(ClaimsPrincipal user)
        {
            return user.Claims.Select(c => $"{c.Type}:{c.Value}").ToList();
        }

        /// <summary>
        /// Function used to extract the roles from the user.
        /// Can be overridden in derived classes to extract the roles in a different way.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The list of roles from the values of claims of type Role</returns>
        protected virtual string[] GetUserRoles(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();
        }

        /// <summary>
        /// Add a generic rule for an action to the available actions.
        /// Generic rules can be checked without an instance of T.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="ruleText"></param>
        /// <exception cref="ArgumentException"></exception>
        protected void AddGenericRule(string actionName, string ruleText)
        {
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("Action name cannot be null or empty", nameof(actionName));

            if (_availableActions.ContainsKey(actionName))
                throw new ArgumentException($"Action {actionName} already exists", nameof(actionName));

            var action = new AuthorizationAction(actionName, ruleText);
            _availableActions[actionName] = action;
        }

        /// <summary>
        /// Add a rule for an action that requires an instance of T to be checked.
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="ruleText"></param>
        /// <exception cref="ArgumentException"></exception>
        protected void AddInstanceRule(string actionName, string ruleText)
        {
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("Action name cannot be null or empty", nameof(actionName));

            if (_availableActions.ContainsKey(actionName))
                throw new ArgumentException($"Action {actionName} already exists", nameof(actionName));

            var action = new AuthorizationAction(actionName, ruleText, isInstanceAction: true);
            _availableActions[actionName] = action;
        }

        /// <summary>
        /// Returns an expression that can be used to filter IQueryable
        /// list of resource type T.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Expression<Func<T, bool>> GetAuthorizationExpression(ClaimsPrincipal user, string actionName)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(actionName)) throw new ArgumentException("Action cannot be null or empty.", nameof(actionName));

            if (!_availableActions.TryGetValue(actionName, out var actionInfo))
                throw new NotRecognizedActionException(typeof(T), actionName);
            // Initialize user variables
            var interpreter = CreateInterpreter(user);

            // Parse the rule into an expression
            try
            {
                // Parse the expression
                var parsedExpression = interpreter.ParseAsExpression<Func<T, bool>>(actionInfo.Rule, "Resource");

                return parsedExpression;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error parsing authorization rule into an expression.", ex);
            }
        }
    }
}
