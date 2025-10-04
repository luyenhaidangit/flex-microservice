using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class WorkflowRequestRepository : RepositoryBase<WorkflowRequest, long, WorkflowDbContext>, IWorkflowRequestRepository
    {
        private readonly WorkflowDbContext _context;
        public WorkflowRequestRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public IQueryable<WorkflowRequest> QueryPending()
        {
            return _context.WorkflowRequests.AsNoTracking().Where(x => x.Status == Infrastructure.Workflow.Constants.RequestStatus.Unauthorised);
        }

        public Task<WorkflowRequest?> GetPendingByIdAsync(long requestId, CancellationToken ct = default)
        {
            return _context.WorkflowRequests
                .FirstOrDefaultAsync(x => x.Id == requestId && x.Status == Infrastructure.Workflow.Constants.RequestStatus.Unauthorised, ct);
        }
    }
}
