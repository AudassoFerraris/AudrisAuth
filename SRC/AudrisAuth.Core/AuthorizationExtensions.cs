using System;
using System.Collections.Generic;
using System.Text;

namespace AudrisAuth
{
    public static class AuthorizationExtensions
    {
        /// <summary>
        /// Throws a NotRecognizedActionException for the specified action if it is not recognized.
        /// </summary>
        /// <typeparam name="T">The type for which the authorization is checked.</typeparam>
        /// <param name="authorization">The authorization instance.</param>
        /// <param name="action">The action that is invalid.</param>
        /// <exception cref="NotRecognizedActionException">Thrown if the action is not recognized.</exception>
        public static void ThrowNotRecognizedActionException<T>(this IAuthorization<T> authorization, string action)
        {
            throw new NotRecognizedActionException(typeof(T), action);
        }
    }
}
