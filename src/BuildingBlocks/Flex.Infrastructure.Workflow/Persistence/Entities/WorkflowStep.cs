using Flex.Contracts.Domains;

namespace Flex.Infrastructure.Workflow.Persistence.Entities;

public class WorkflowStep : EntityBase<long>
{
    public long RequestId { get; set; }
    public int Level { get; set; }
    public int Order { get; set; }
    public string? ApproverRole { get; set; }
    public string? ApproverUser { get; set; }
    public string Decision { get; set; } = string.Empty; // e.g., "APP"/"REJ" or use constants later
    public string? DecidedBy { get; set; }
    public DateTime? DecidedAt { get; set; }
    public string? Comment { get; set; }
}
