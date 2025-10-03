using Flex.Infrastructure.Workflow.Abstractions.Enums;

namespace Flex.Infrastructure.Workflow.Persistence.Entities;

public class WorkflowRequest
{
    public long Id { get; set; }
    public string WorkflowType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public WorkflowStatus Status { get; set; } = WorkflowStatus.Pending;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string PayloadJson { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public string? CorrelationId { get; set; }
    public string? MetadataJson { get; set; }
}

