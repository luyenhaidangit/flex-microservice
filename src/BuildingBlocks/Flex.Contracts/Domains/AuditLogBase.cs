using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Contracts.Domains
{
    public abstract class AuditLogBase : EntityBase<long>
    {
        [Required]
        [Column("OPERATION", TypeName = "VARCHAR2(50)")]
        public string Operation { get; set; } = string.Empty;

        [Column("OLD_VALUE", TypeName = "CLOB")]
        public string? OldValue { get; set; }

        [Column("NEW_VALUE", TypeName = "CLOB")]
        public string? NewValue { get; set; }

        [Required]
        [Column("REQUESTED_BY", TypeName = "VARCHAR2(100)")]
        public string RequestedBy { get; set; } = string.Empty;

        [Required]
        [Column("APPROVED_BY", TypeName = "VARCHAR2(100)")]
        public string ApprovedBy { get; set; } = string.Empty;

        [Required]
        [Column("AUDIT_DATE")]
        public DateTime AuditDate { get; set; } = DateTime.UtcNow;
    }
}
