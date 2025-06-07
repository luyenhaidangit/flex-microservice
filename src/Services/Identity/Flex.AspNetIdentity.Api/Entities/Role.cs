using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.AspNetIdentity.Api.Entities
{
    public class Role : IdentityRole<long>
    {
        [Required]
        [Column("CODE")]
        public string Code { get; set; }

        [Column("DESCRIPTION")]
        public string? Description { get; set; } = string.Empty;

        [Column("IS_ACTIVE")]
        public bool? IsActive { get; set; } = true;

        public Role()
        {
        }

        public Role(string roleName, string code)
        {
            Name = roleName;
            NormalizedName = roleName.ToUpper();
            ConcurrencyStamp = Guid.NewGuid().ToString();
            Code = code;
        }
    }
}
