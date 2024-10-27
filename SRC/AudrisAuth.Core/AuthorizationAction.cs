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
        /// <param name="name">name of the action</param>
        /// <param name="rule">rule for this action</param>
        /// <param name="isInstanceAction">if true the action can be checked against a value of T</param>
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

        /// <summary>
        /// Check if the object is equal to this instance of AuthorizationAction
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as AuthorizationAction);
        }

        /// <summary>
        /// Equality check for the AuthorizationAction class based on the Name, IsInstanceAction and Rule properties
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(AuthorizationAction other)
        {
            return other != null &&
                   Name == other.Name &&
                   IsInstanceAction == other.IsInstanceAction &&
                   Rule == other.Rule;
        }

        /// <summary>
        /// Override of the GetHashCode method for the AuthorizationAction class
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = Name.GetHashCode();
            hashCode = hashCode * -1521134295 + IsInstanceAction.GetHashCode();
            hashCode = hashCode * -1521134295 + Rule.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Override of the ToString method for the AuthorizationAction class
        /// </summary>
        /// <returns>The name property</returns>
        public override string ToString() => Name;
    }

}
