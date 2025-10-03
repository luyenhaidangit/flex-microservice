using Flex.Infrastructure.Workflow.Persistence.Entities;

namespace Flex.Infrastructure.Workflow.Core.Models;

public class WorkflowContext
{
    public WorkflowRequest Request { get; init; } = default!;
    public IReadOnlyList<WorkflowStep> Steps { get; init; } = Array.Empty<WorkflowStep>();
}

