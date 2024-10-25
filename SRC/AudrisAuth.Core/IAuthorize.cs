using System.Security.Claims;

namespace AudrisAuth
{
    /// <summary>
    /// Main interface for authorization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAuthorize<T>
    {
        /// <summary>
        /// Check if user can perform a generic action
        /// related to the resource type T
        /// </summary>
        /// <param name="user"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        bool Can(ClaimsPrincipal user, string action);

        /// <summary>
        /// Check if user can perform a specific action
        /// on an instance of resource type T
        /// </summary>
        /// <param name="user"></param>
        /// <param name="resource"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        bool Can(ClaimsPrincipal user, T resource, string action);
    }
}
