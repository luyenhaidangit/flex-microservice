using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class ApprovalRequestRepository : RepositoryBase<ApprovalRequest, long, WorkflowDbContext>, IApprovalRequestRepository
    {
        private readonly WorkflowDbContext _context;
        public ApprovalRequestRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public IQueryable<ApprovalRequest> QueryPending()
        {
            return _context.ApprovalRequests.AsNoTracking().Where(x => x.Status == Infrastructure.Workflow.Constants.RequestStatus.Unauthorised);
        }

        public Task<ApprovalRequest?> GetPendingByIdAsync(long requestId, CancellationToken ct = default)
        {
            return _context.ApprovalRequests
                .FirstOrDefaultAsync(x => x.Id == requestId && x.Status == Infrastructure.Workflow.Constants.RequestStatus.Unauthorised, ct);
        }
    }
}

