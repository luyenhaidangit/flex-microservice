using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Workflow.Persistence;
using Flex.Infrastructure.Workflow.Persistence.Entities;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IWorkflowRequestRepository : IRepositoryBase<WorkflowRequest, long, WorkflowDbContext>
    {
        Task<bool> ExistsByCorrelationIdAsync(string correlationId, CancellationToken ct = default);
        Task<WorkflowRequest?> GetByIdAsync(long id, CancellationToken ct = default);
    }
}

