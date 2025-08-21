using Microsoft.AspNetCore.Identity;

namespace Flex.AspNetIdentity.Api.Entities
{
    public class User : IdentityUser<long>
    {
        public string FullName { get; set; } = default!;
        public long BranchId { get; set; }
    }
}
