using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Flex.Contracts.Domains;

namespace Flex.System.Api.Entities
{
    [Table("USER_REQUEST_DATA")]
    public class UserRequestData : EntityBase<long>
    {
        [Required]
        [Column("REQUEST_ID")]
        public long RequestId { get; set; }

        [Required] 
        [Column("USERNAME", TypeName = "VARCHAR2(50)")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [Column("PASSWORD_HASH", TypeName = "VARCHAR2(100)")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("FULL_NAME", TypeName = "VARCHAR2(250)")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column("BRANCH_ID")]
        public long BranchId { get; set; }
        
        [Column("EMAIL", TypeName = "VARCHAR2(256)")]
        public string? Email { get; set; }

        [Column("PHONE_NUMBER", TypeName = "VARCHAR2(50)")]
        public string? PhoneNumber { get; set; }
    }
}
