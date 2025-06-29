using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.AspNetIdentity.Api.Entities
{
    [Table("USERS")]
    public class User : IdentityUser<long>
    {
        // UserName and PasswordHash are inherited from IdentityUser

        [StringLength(250)]
        [Column("FULL_NAME")]
        public string? FullName { get; set; } = string.Empty;

        [Column("BRANCH_ID")] 
        public long? BranchId { get; set; }
    }
}
