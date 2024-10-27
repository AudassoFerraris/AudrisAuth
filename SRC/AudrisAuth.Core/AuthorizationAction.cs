using System;

namespace AudrisAuth
{
    /// <summary>
    /// AuthorizationAction that can be performed on a resource
    /// </summary>
    public class AuthorizationAction : IEquatable<AuthorizationAction>
    {
        /// <summary>
        /// Name of the action
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// True if the action can be only applied at instance level
        /// </summary>
        public bool IsInstanceAction { get; }

        /// <summary>
        /// Rule to be applied to the action
        /// </summary>
        public string Rule { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationAction"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isInstanceAction"></param>
        /// <exception cref="ArgumentException">If name or rule are null or empty</exception>
        public AuthorizationAction(string name, string rule, bool isInstanceAction = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name cannot be null or empty", nameof(name));

            if (string.IsNullOrWhiteSpace(rule))
                throw new ArgumentException("Rule cannot be null or empty", nameof(rule));

            Name = name;
            Rule = rule;
            IsInstanceAction = isInstanceAction;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AuthorizationAction);
        }

        public bool Equals(AuthorizationAction other)
        {
            return other != null &&
                   Name == other.Name &&
                   IsInstanceAction == other.IsInstanceAction &&
                   Rule == other.Rule;
        }

        public override int GetHashCode()
        {
            var hashCode = Name.GetHashCode();
            hashCode = hashCode * -1521134295 + IsInstanceAction.GetHashCode();
            hashCode = hashCode * -1521134295 + Rule.GetHashCode();
            return hashCode;
        }

        public override string ToString() => Name;
    }

}
