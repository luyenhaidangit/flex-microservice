namespace Flex.AspNetIdentity.Api.Models
{
    public class CreateUserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public long? BranchId { get; set; }
    }
}
