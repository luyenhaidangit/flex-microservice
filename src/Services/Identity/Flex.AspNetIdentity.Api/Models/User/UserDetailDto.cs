using Flex.AspNetIdentity.Api.Models.Branch;

namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UserDetailDto : UserPagingDto
    {
        public List<string> Roles { get; set; } = new();

        public BranchLookupDto Branch { get; set; } = new(0, string.Empty);
    }
}