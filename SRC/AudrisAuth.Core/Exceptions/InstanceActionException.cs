using System;

namespace AudrisAuth.Exceptions
{
    /// <summary>
    /// Exception raised when an action for an instance is checked 
    /// against the authorization class without a specific instance
    /// </summary>
    public class InstanceActionException : Exception
    {
        /// <summary>
        /// Gets the action that was not recognized
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// Gets the type for which the action was not recognized
        /// </summary>
        public Type ReferenceType { get; }

        public InstanceActionException(Type referenceType, string action) : base($"Action '{action}' cannot be checked without a specific instance of type {referenceType.Name}")
        {
            ReferenceType = referenceType;
            Action = action;
        }
    }
}
