namespace Flex.AspNetIdentity.Api.Models.User
{
    /// <summary>
    /// Request model for creating user directly (admin only, no approval needed)
    /// </summary>
    public class CreateUserDirectRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long BranchId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool SendPasswordEmail { get; set; } = true;
    }
}
