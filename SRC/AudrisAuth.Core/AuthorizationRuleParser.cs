using DynamicExpresso;

using System;
using System.Linq.Expressions;
using System.Security.Claims;

namespace AudrisAuth
{
    public class AuthorizationRuleParser<T>
    {
        private readonly Interpreter _interpreter;

        public AuthorizationRuleParser()
        {
            _interpreter = new Interpreter();

            _interpreter.SetFunction(
                "IsInRole",
                (Func<ClaimsPrincipal, string, bool>)((user, role) => user.IsInRole(role))
            );

            _interpreter.SetFunction(
                "HasClaim",
                (Func<ClaimsPrincipal, string, string, bool>)((user, type, value) => user.HasClaim(role => user.HasClaim(type, value)))
            );
        }

        public void RegisterInstanceFunction(string name, Func<ClaimsPrincipal, T, bool> function)
        {
            _interpreter.SetFunction(name, function);
        }

        public GenericAuthorizationRule ParseGenericRule(string rule)
        {
            var lambda = _interpreter.ParseAsExpression<Func<ClaimsPrincipal, bool>>(rule, "user");
            return new GenericAuthorizationRule(lambda);
        }

        public InstanceAuthorizationRule<T> ParseInstanceRule(string rule)
        {
            var lambda = _interpreter.ParseAsExpression<Func<ClaimsPrincipal, T, bool>>(rule, "user", "item" );
            return new InstanceAuthorizationRule<T>(lambda);
        }
    }
}
