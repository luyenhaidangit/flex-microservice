using Flex.Shared.SeedWork.Workflow;

namespace Flex.Workflow.Api.Entities
{
    // Centralized approval request header for cross-domain orchestration
    public class ApprovalRequest : RequestBase<string>
    {
        // Business domain/module that raised this request (e.g., IDENTITY, SYSTEM, PAYMENTS)
        public string Domain { get; set; } = string.Empty;
        // Workflow definition code this request binds to (e.g., APPROVAL_ROLE, BRANCH_MD)
        public string WorkflowCode { get; set; } = string.Empty;
        // Natural business key (e.g., role code) to enforce idempotency
        public string? BusinessId { get; set; }
        // Optional correlation id for external systems
        public string? CorrelationId { get; set; }
    }
}
