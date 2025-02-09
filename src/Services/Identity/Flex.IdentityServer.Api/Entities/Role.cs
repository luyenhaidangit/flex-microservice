using Microsoft.AspNetCore.Identity;

namespace Flex.IdentityServer.Api.Entities
{
    public class Role : IdentityRole<long>
    {
        public Role()
        {
        }

        public Role(string roleName)
        {
            Name = roleName;
            NormalizedName = roleName.ToUpper();
            ConcurrencyStamp = Guid.NewGuid().ToString();
        }
    }
}
