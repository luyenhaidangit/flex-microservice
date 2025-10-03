using Flex.Contracts.Domains;

namespace Flex.Infrastructure.Workflow.Persistence.Entities;

public class WorkflowRequest : EntityBase<long>
{
    public string WorkflowType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Use RequestStatus constants
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string PayloadJson { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string? CorrelationId { get; set; }
    public string? MetadataJson { get; set; }
}
