using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.AspNetIdentity.Api.Entities
{
    [Table("ROLES")]
    public class Role : IdentityRole<long>
    {
        [Required]
        [Column("CODE")]
        public string Code { get; set; }

        [Column("DESCRIPTION")]
        public string? Description { get; set; } = string.Empty;

        [Column("IS_ACTIVE")]
        public bool? IsActive { get; set; } = true;

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("LAST_UPDATED")]
        public DateTime? LastUpdated { get; set; }

        [Column("STATUS")]
        public string Status { get; set; } = "APPROVED";

        [Column("VERSION")]
        public int Version { get; set; } = 1;

        [Column("PARENT_ID")]
        public long? ParentId { get; set; }

        [Column("MAKER_ID")]
        public string? MakerId { get; set; }

        [Column("CHECKER_ID")]
        public string? CheckerId { get; set; }

        [Column("REQUEST_ID")]
        public long? RequestId { get; set; }

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
