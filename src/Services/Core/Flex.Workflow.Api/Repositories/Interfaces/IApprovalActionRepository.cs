using Flex.Contracts.Domains.Interfaces;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;

namespace Flex.Workflow.Api.Repositories.Interfaces
{
    public interface IApprovalActionRepository : IRepositoryBase<ApprovalAction, long, WorkflowDbContext>
    {
        Task<List<ApprovalAction>> GetByRequestAsync(long requestId, CancellationToken ct = default);
    }
}

