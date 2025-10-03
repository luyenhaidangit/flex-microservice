namespace Flex.Infrastructure.Workflow.Abstractions.DTOs;

public class WorkflowRequestDto
{
    public long Id { get; set; }
    public string WorkflowType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public string? Comment { get; set; }
}
