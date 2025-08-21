namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UserDetailDto : UserPagingDto
    {
        public List<string> Roles { get; set; } = new();
    }
}
