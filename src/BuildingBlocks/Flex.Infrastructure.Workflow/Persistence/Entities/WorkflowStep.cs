using Flex.Infrastructure.Workflow.Abstractions.Enums;

namespace Flex.Infrastructure.Workflow.Persistence.Entities;

public class WorkflowStep
{
    public long Id { get; set; }
    public long RequestId { get; set; }
    public int Level { get; set; }
    public int Order { get; set; }
    public string? ApproverRole { get; set; }
    public string? ApproverUser { get; set; }
    public Decision Decision { get; set; } = Decision.None;
    public string? DecidedBy { get; set; }
    public DateTime? DecidedAt { get; set; }
    public string? Comment { get; set; }
}

