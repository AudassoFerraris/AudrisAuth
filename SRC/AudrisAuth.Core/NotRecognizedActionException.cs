using System;

namespace AudrisAuth
{
    /// <summary>
    /// Exception raised when an action is not recognized for a given type
    /// by the authorization class
    /// </summary>
    public class NotRecognizedActionException : Exception
    {
        /// <summary>
        /// Gets the type for withc the action was not recognized
        /// </summary>
        public Type ReferenceType { get; }

        /// <summary>
        /// Gets the action that was not recognized
        /// </summary>
        public string Action { get; }

        public NotRecognizedActionException(Type referenceType, string action) : base($"Action '{action}' not recognized for type '{referenceType.Name}'")
        {
            ReferenceType = referenceType;
            Action = action;
        }
    }
}
