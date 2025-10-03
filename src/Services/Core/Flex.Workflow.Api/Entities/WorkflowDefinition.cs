using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Workflow.Api.Entities
{
    public class WorkflowDefinition : EntityBase<long>
    {
        [Required]
        [Column("CODE", TypeName = "VARCHAR2(100)")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Column("NAME", TypeName = "NVARCHAR2(200)")]
        public string Name { get; set; } = string.Empty;

        [Column("VERSION")]
        public int Version { get; set; } = 1;

        [Column("IS_ACTIVE")]
        public bool IsActive { get; set; } = true;

        // JSON: step definitions, routing strategies, quorum, parallel groups
        [Column("STEPS", TypeName = "CLOB")]
        public string Steps { get; set; } = string.Empty;

        // JSON: policy rules (simple expression or OPA-like inputs)
        [Column("POLICY", TypeName = "CLOB")]
        public string? Policy { get; set; }

        [Column("DESCRIPTION", TypeName = "NVARCHAR2(500)")]
        public string? Description { get; set; }

        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }

        [Column("UPDATED_BY", TypeName = "VARCHAR2(100)")]
        public string? UpdatedBy { get; set; }
    }
}

