using Flex.Shared.SeedWork.Workflow;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Workflow.Api.Entities
{
    // Centralized approval request header for cross-domain orchestration
    public class ApprovalRequest : RequestBase<long>
    {
        // Business domain/module that raised this request (e.g., IDENTITY, SYSTEM, PAYMENTS)
        [Required]
        [Column("DOMAIN", TypeName = "VARCHAR2(50)")]
        public string Domain { get; set; } = string.Empty;

        // Workflow definition code this request binds to (e.g., APPROVAL_ROLE, BRANCH_MD)
        [Required]
        [Column("WORKFLOW_CODE", TypeName = "VARCHAR2(100)")]
        public string WorkflowCode { get; set; } = string.Empty;

        // Natural business key (e.g., role code) to enforce idempotency
        [Column("BUSINESS_ID", TypeName = "VARCHAR2(200)")]
        public string? BusinessId { get; set; }

        // Optional correlation id for external systems
        [Column("CORRELATION_ID", TypeName = "VARCHAR2(100)")]
        public string? CorrelationId { get; set; }
    }
}

