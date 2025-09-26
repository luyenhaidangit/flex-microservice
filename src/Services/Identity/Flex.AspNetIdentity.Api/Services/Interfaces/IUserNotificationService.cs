namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    /// <summary>
    /// Service for sending user notifications (email, SMS, etc.)
    /// </summary>
    public interface IUserNotificationService
    {
        /// <summary>
        /// Sends password notification email to user
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="userName">Username</param>
        /// <param name="temporaryPassword">Temporary password</param>
        /// <param name="fullName">User full name</param>
        /// <returns>Task representing the async operation</returns>
        Task SendPasswordNotificationAsync(string email, string userName, string temporaryPassword, string? fullName = null);

        /// <summary>
        /// Sends password change required notification
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="userName">Username</param>
        /// <param name="fullName">User full name</param>
        /// <returns>Task representing the async operation</returns>
        Task SendPasswordChangeRequiredNotificationAsync(string email, string userName, string? fullName = null);

        /// <summary>
        /// Sends account activation notification
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="userName">Username</param>
        /// <param name="activationLink">Account activation link</param>
        /// <param name="fullName">User full name</param>
        /// <returns>Task representing the async operation</returns>
        Task SendAccountActivationNotificationAsync(string email, string userName, string activationLink, string? fullName = null);
    }
}