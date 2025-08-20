using Microsoft.AspNetCore.Identity;

namespace Flex.AspNetIdentity.Api.Entities
{
    public class User : IdentityUser<long>
    {
        // UserName and PasswordHash are inherited from IdentityUser
        public string? FullName { get; set; } = string.Empty;
        public long? BranchId { get; set; }
    }
}
