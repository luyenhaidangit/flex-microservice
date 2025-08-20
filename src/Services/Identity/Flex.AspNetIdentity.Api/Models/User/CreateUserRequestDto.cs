namespace Flex.AspNetIdentity.Api.Models.User
{
    public class CreateUserRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long? BranchId { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string> RoleCodes { get; set; } = new();
    }
}


