namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UserPagingDto
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
