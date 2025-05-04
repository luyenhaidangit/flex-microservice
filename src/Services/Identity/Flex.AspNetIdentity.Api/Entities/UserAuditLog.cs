using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Flex.Contracts.Domains;

namespace Flex.System.Api.Entities
{
    [Table("USER_AUDIT_LOG")]
    public class UserAuditLog : AuditLogBase
    {
        [Required]
        [Column("USER_ID")]
        public long UserId { get; set; }
    }
}
