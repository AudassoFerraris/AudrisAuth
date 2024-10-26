using System;
using System.Linq.Expressions;
using System.Security.Claims;

namespace AudrisAuth
{
    /// <summary>
    /// Represents a generic authorization rule based on a ClaimsPrincipal.
    /// </summary>
    public class GenericAuthorizationRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericAuthorizationRule"/> class.
        /// </summary>
        /// <param name="rule">The expression defining the rule.</param>
        /// <exception cref="ArgumentNullException">Thrown when the rule is null.</exception>
        public GenericAuthorizationRule(Expression<Func<ClaimsPrincipal, bool>> rule)
        {
            Rule = rule ?? throw new ArgumentNullException(nameof(rule));
            _compiledRule = new Lazy<Func<ClaimsPrincipal, bool>>(() => Rule.Compile(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// Gets the expression that defines the rule.
        /// </summary>
        public Expression<Func<ClaimsPrincipal, bool>> Rule { get; }

        readonly private Lazy<Func<ClaimsPrincipal, bool>> _compiledRule;
        /// <summary>
        /// Gets the compiled version of the rule.
        /// </summary>
        public Func<ClaimsPrincipal, bool> CompiledRule => _compiledRule.Value;
    }

}
