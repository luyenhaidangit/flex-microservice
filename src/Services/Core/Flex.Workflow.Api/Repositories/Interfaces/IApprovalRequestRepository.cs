using Flex.Contracts.Domains.Interfaces;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IApprovalRequestRepository : IRepositoryBase<ApprovalRequest, long, WorkflowDbContext>
    {
        Task<ApprovalRequest?> GetPendingByIdAsync(long requestId, CancellationToken ct = default);
        IQueryable<ApprovalRequest> QueryPending();
    }
}

