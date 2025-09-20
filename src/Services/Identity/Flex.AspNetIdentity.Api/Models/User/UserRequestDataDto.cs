namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UserRequestDataDto
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public long BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
    }
}
