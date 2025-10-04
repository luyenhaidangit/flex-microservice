using Flex.Contracts.Domains;

namespace Flex.Workflow.Api.Entities
{
    public class WorkflowDefinition : EntityBase<long>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Version { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        // Draft | Active | Deprecated
        public string State { get; set; } = "Draft";
        // JSON: step definitions, routing strategies, quorum, parallel groups
        public string Steps { get; set; } = string.Empty;
        // JSON: policy rules (simple expression or OPA-like inputs)
        public string? Policy { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
