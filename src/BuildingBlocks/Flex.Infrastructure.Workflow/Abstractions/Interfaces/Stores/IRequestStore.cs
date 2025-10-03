using Flex.Infrastructure.Workflow.Persistence.Entities;

namespace Flex.Infrastructure.Workflow.Abstractions.Interfaces.Stores;

public interface IRequestStore
{
    Task<WorkflowRequest?> GetAsync(long id, CancellationToken ct = default);
    Task<long> CreateAsync(WorkflowRequest request, CancellationToken ct = default);
    Task UpdateAsync(WorkflowRequest request, CancellationToken ct = default);
    IQueryable<WorkflowRequest> Query();
}

