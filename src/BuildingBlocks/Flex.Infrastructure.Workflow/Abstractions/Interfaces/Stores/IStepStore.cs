using Flex.Infrastructure.Workflow.Persistence.Entities;

namespace Flex.Infrastructure.Workflow.Abstractions.Interfaces.Stores;

public interface IStepStore
{
    Task AddRangeAsync(IEnumerable<WorkflowStep> steps, CancellationToken ct = default);
    Task<IReadOnlyList<WorkflowStep>> GetByRequestAsync(long requestId, CancellationToken ct = default);
    Task UpdateAsync(WorkflowStep step, CancellationToken ct = default);
}

