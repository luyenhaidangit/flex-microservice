namespace Flex.AspNetIdentity.Api.Models.User
{
    public class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public long? BranchId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}