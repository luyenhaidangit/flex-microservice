namespace Flex.AspNetIdentity.Api.Models.User
{
    /// <summary>
    /// Request model for changing password (required on first login)
    /// </summary>
    public class ChangePasswordRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
