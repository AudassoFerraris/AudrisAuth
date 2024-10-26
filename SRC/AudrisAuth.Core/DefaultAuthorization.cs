using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.Claims;



namespace AudrisAuth
{
    /// <summary>
    /// Default implementation of IAuthorization interface
    /// that use a dictionary of Predicate to check each action
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class DefaultAuthorization<T> : IAuthorization<T>
    {
        protected Dictionary<string, GenericAuthorizationRule> _genericActions = new Dictionary<string, GenericAuthorizationRule>();
        protected Dictionary<string, InstanceAuthorizationRule<T>> _instanceActions = new Dictionary<string, InstanceAuthorizationRule<T>>();
        protected Dictionary<string, AuthorizationAction> _availableActions = new Dictionary<string, AuthorizationAction>();

        public DefaultAuthorization()
        {
        }

        public bool Can(ClaimsPrincipal user, string action)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var actionInfo = ResolveAction(action, true);
            if (!_genericActions.TryGetValue(action, out var rule))
            {
                return false;
            }
            var result = rule.CompiledRule(user);
            return result;
        }

        public bool Can(ClaimsPrincipal user, T resource, string action)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }
            var actionInfo = ResolveAction(action, false);
            if (actionInfo.IsInstanceAction)
            {
                if (!_instanceActions.TryGetValue(action, out var rule))
                {
                    return false;
                }
                var result = rule.CompiledRule(user, resource);
                return result;
            } else
            {
                if (!_genericActions.TryGetValue(action, out var rule))
                {
                    return false;
                }
                var result = rule.CompiledRule(user);
                return result;
            }
        }

        /// <summary>
        /// Check for the action and return the AuthorizationAction
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onlyGenericActions"></param>
        /// <returns></returns>
        /// <exception cref="InstanceActionException">When action is not found</exception>
        /// <exception cref="NotRecognizedActionException">If the action is managed but is referred to an instance</exception>
        protected AuthorizationAction ResolveAction(string action, bool onlyGenericActions)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (!_availableActions.TryGetValue(action, out var actionInfo))
            {
                this.ThrowNotRecognizedActionException(action);
            }
            if (onlyGenericActions  && actionInfo.IsInstanceAction)
            {
                throw new InstanceActionException(typeof(T), action);
            }
            return actionInfo;
        }


        public IEnumerable<AuthorizationAction> GetAvailableActions()
        {
            return _availableActions.Values;
        }
    }
}
