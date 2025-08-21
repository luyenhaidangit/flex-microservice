namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UserListItemDto
    {
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long? BranchId { get; set; }
        public bool IsLocked { get; set; }
        public bool IsActive { get; set; }
    }
}
