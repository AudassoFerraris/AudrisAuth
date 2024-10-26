using System;
using System.Collections.Generic;
using System.Text;

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
        public bool IsInstanceAction { get; }  // True se può essere applicata a livello di istanza

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationAction"/> class.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isInstanceAction"></param>
        /// <exception cref="ArgumentException"></exception>
        public AuthorizationAction(string name, bool isInstanceAction = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Action name cannot be null or empty", nameof(name));

            Name = name;
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
                   IsInstanceAction == other.IsInstanceAction;
        }

        public override int GetHashCode()
        {
            var hashCode = Name.GetHashCode();
            hashCode = hashCode * -1521134295 + IsInstanceAction.GetHashCode();
            return hashCode;
        }

        public override string ToString() => Name;
    }

}
