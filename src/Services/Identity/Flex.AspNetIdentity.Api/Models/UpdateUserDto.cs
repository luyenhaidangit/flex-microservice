namespace Flex.AspNetIdentity.Api.Models
{
    public class UpdateUserDto
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? FullName { get; set; }
        public long? BranchId { get; set; }
    }
}
