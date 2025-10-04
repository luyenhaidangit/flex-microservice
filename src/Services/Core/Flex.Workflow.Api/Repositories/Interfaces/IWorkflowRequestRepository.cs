using Flex.Contracts.Domains.Interfaces;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IWorkflowRequestRepository : IRepositoryBase<WorkflowRequest, long, WorkflowDbContext>
    {
        Task<WorkflowRequest?> GetPendingByIdAsync(long requestId, CancellationToken ct = default);
        IQueryable<WorkflowRequest> QueryPending();
    }
}
