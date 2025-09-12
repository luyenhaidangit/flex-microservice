namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long BranchId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}