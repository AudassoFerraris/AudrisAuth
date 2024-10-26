using System;
using System.Linq.Expressions;
using System.Security.Claims;

namespace AudrisAuth
{
    /// <summary>
    /// Represents an authorization rule based on a ClaimsPrincipal and an instance of a type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InstanceAuthorizationRule<T>
    {
        /// <summary>
        /// Gets the expression that defines the rule.
        /// </summary>
        public Expression<Func<ClaimsPrincipal, T, bool>> Rule { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceAuthorizationRule{T}"/> class.
        /// </summary>
        /// <param name="rule">The expression defining the rule</param>
        /// <exception cref="ArgumentNullException">Thrown when the rule is null</exception>
        public InstanceAuthorizationRule(Expression<Func<ClaimsPrincipal, T, bool>> rule)
        {
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            _compiledRule = new Lazy<Func<ClaimsPrincipal, T, bool>>(() => Rule.Compile(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        readonly private Lazy<Func<ClaimsPrincipal, T, bool>> _compiledRule;
        /// <summary>
        /// Gets the compiled version of the rule.
        /// </summary>
        public Func<ClaimsPrincipal, T, bool> CompiledRule => _compiledRule.Value;
    }

}
