using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using AudrisAuth.Exceptions;

using DynamicExpresso;

namespace AudrisAuth
{
    public abstract class DefaultAuthorization<T> : IAuthorization<T>
    {
        protected readonly Dictionary<string, AuthorizationAction> _availableActions = new Dictionary<string, AuthorizationAction>();

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

            // Register functions
            interpreter.SetFunction("HasRole", (Func<string, bool>)(role => userRoles.Contains(role)));
            interpreter.SetFunction("HasClaim", (Func<string, string, bool>)((type, value) =>
                userClaims.TryGetValue(type, out var values) && values.Contains(value)));

            ConfigureInterpreter(interpreter);


            return interpreter;
        }

        /// <summary>
        /// Allow to configure the interpreter with custom functions
        /// in the derived class
        /// </summary>
        /// <param name="interpreter"></param>
        protected virtual void ConfigureInterpreter(Interpreter interpreter)
        {
            
        }

        protected virtual string GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        protected virtual Dictionary<string, List<string>> GetClaims(ClaimsPrincipal user)
        {
            return user.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToList());
        }

        protected virtual string[] GetUserRoles(ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();
        }

        // Method to add a generic rule
        protected void AddGenericRule(string actionName, string ruleText)
        {
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("Action name cannot be null or empty", nameof(actionName));

            if (_availableActions.ContainsKey(actionName))
                throw new ArgumentException($"Action {actionName} already exists", nameof(actionName));

            var action = new AuthorizationAction(actionName, ruleText);
            _availableActions[actionName] = action;
        }

        // Method to add an instance rule
        protected void AddInstanceRule(string actionName, string ruleText)
        {
            if (string.IsNullOrWhiteSpace(actionName))
                throw new ArgumentException("Action name cannot be null or empty", nameof(actionName));

            if (_availableActions.ContainsKey(actionName))
                throw new ArgumentException($"Action {actionName} already exists", nameof(actionName));

            var action = new AuthorizationAction(actionName, ruleText, isInstanceAction: true);
            _availableActions[actionName] = action;
        }
    }
}
