namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UserRequestDataDto
    {
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long? BranchId { get; set; }
        public string? BranchName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
