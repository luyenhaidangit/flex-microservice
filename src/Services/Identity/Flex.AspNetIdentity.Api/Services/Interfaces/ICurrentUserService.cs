namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface ICurrentUserService
    {
        /// <summary>
        /// Checks if the current user is authenticated
        /// </summary>
        /// <returns>True if authenticated, false otherwise</returns>
        bool IsAuthenticated();

        /// <summary>
        /// Gets the username of the currently authenticated user
        /// </summary>
        /// <returns>The username or null if not authenticated</returns>
        string? GetCurrentUsername();
    }
} 