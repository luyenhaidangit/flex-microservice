using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.System.Api.Entities
{
    [Table("BRANCH_AUDIT_LOG")]
    public class BranchAuditLog : EntityBase<long>
    {
        [Required]
        [Column("ENTITY_ID")]
        public long EntityId { get; set; }               // Branch ID

        [Required]
        [Column("OPERATION", TypeName = "VARCHAR2(20)")]
        public string Operation { get; set; } = default!;  // CREATE / UPDATE / DELETE

        [Column("OLD_VALUE", TypeName = "CLOB")]
        public string? OldValue { get; set; }

        [Column("NEW_VALUE", TypeName = "CLOB")]
        public string? NewValue { get; set; }

        [Required]
        [Column("USER_ID", TypeName = "VARCHAR2(100)")]
        public string UserId { get; set; } = default!;

        [Required]
        [Column("LOG_DATE")]
        public DateTime LogDate { get; set; } = DateTime.UtcNow;
    }
}
