using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using Flex.Shared.Constants.Common;

namespace Flex.AspNetIdentity.Api.Entities
{
    [Table("ROLES")]
    public class Role : IdentityRole<long>
    {
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string? Description { get; set; } = string.Empty;
        public string Status { get; set; } = StatusConstant.Approved;

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
