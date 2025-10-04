using Flex.Contracts.Domains;

namespace Flex.Workflow.Api.Entities
{
    // Maker–Checker skeleton for publishing WorkflowDefinition
    public class WorkflowDefinitionPublishRequest : EntityBase<long>
    {
        public string Code { get; set; } = string.Empty;
        public int Version { get; set; }
        public string MakerId { get; set; } = string.Empty;
        public string? CheckerId { get; set; }
        public string Status { get; set; } = "UNA"; // UNA | AUT | REJ
        public string? RequestComment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
    }
}

