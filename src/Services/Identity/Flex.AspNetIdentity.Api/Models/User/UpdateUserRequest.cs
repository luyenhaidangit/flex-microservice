namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; } = default!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long? BranchId { get; set; }
        public bool? IsActive { get; set; }
        public List<string>? RoleCodes { get; set; }
        public string? Comment { get; set; }
    }
}