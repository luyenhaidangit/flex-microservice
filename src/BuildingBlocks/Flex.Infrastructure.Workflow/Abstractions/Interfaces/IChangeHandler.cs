using Flex.Infrastructure.Workflow.Core.Models;

namespace Flex.Infrastructure.Workflow.Abstractions.Interfaces;

public interface IChangeHandler
{
    Task ApplyAsync(WorkflowContext ctx, CancellationToken ct = default);
}

