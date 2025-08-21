using Microsoft.AspNetCore.Identity;

namespace Flex.AspNetIdentity.Api.Entities
{
    public class Role : IdentityRole<long>
    {
        public string Code { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; } = default!;
        public string Status { get; set; } = default!;

        #region Constructors
        protected Role() {}

        public Role(string roleName, string code)
        {
            Name = roleName;
            NormalizedName = roleName.ToUpper();
            ConcurrencyStamp = Guid.NewGuid().ToString();
            Code = code;
        }
        #endregion
    }
}
