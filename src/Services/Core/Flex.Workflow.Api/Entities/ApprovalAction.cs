using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Workflow.Api.Entities
{
    public class ApprovalAction : EntityBase<long>
    {
        [Column("REQUEST_ID")]
        public long RequestId { get; set; }

        [Required]
        [Column("STEP", TypeName = "NUMBER(10)")]
        public int Step { get; set; }

        [Required]
        [Column("ACTION", TypeName = "VARCHAR2(20)")]
        public string Action { get; set; } = string.Empty; // APPROVE / REJECT / HOLD / CANCEL

        [Required]
        [Column("ACTOR_ID", TypeName = "VARCHAR2(100)")]
        public string ActorId { get; set; } = string.Empty;

        [Column("COMMENT", TypeName = "NVARCHAR2(500)")]
        public string? Comment { get; set; }

        [Column("EVIDENCE_URL", TypeName = "VARCHAR2(500)")]
        public string? EvidenceUrl { get; set; }

        [Required]
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

