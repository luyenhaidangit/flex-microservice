using Flex.Infrastructure.Workflow.Abstractions.Enums;

namespace Flex.Infrastructure.Workflow.Abstractions.DTOs;

public class WorkflowRequestDto
{
    public long Id { get; set; }
    public string WorkflowType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public WorkflowStatus Status { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string? Comment { get; set; }
}

