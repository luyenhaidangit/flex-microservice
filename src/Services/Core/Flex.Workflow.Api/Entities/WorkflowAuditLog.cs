using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Workflow.Api.Entities
{
    public class WorkflowAuditLog : EntityBase<long>
    {
        [Required]
        [Column("REQUEST_ID")]
        public long RequestId { get; set; }

        [Required]
        [Column("EVENT", TypeName = "VARCHAR2(50)")]
        public string Event { get; set; } = string.Empty; // created, submitted, approved, rejected, escalated, expired

        [Required]
        [Column("ACTOR_ID", TypeName = "VARCHAR2(100)")]
        public string ActorId { get; set; } = string.Empty;

        [Column("METADATA", TypeName = "CLOB")]
        public string? Metadata { get; set; }

        [Column("PREV_HASH", TypeName = "VARCHAR2(128)")]
        public string? PrevHash { get; set; }

        [Column("CURR_HASH", TypeName = "VARCHAR2(128)")]
        public string CurrHash { get; set; } = string.Empty;

        [Required]
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

