using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Workflow.Api.Entities;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Repositories
{
    public class WorkflowAuditLogRepository : RepositoryBase<WorkflowAuditLog, long, WorkflowDbContext>, IWorkflowAuditLogRepository
    {
        private readonly WorkflowDbContext _context;
        public WorkflowAuditLogRepository(WorkflowDbContext dbContext, IUnitOfWork<WorkflowDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public async Task<WorkflowAuditLog?> GetLastHashAsync(long requestId, CancellationToken ct = default)
        {
            return await _context.WorkflowAuditLogs.AsNoTracking()
                .Where(x => x.RequestId == requestId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync(ct);
        }

        public Task<List<WorkflowAuditLog>> GetByRequestAsync(long requestId, CancellationToken ct = default)
        {
            return _context.WorkflowAuditLogs.AsNoTracking()
                .Where(x => x.RequestId == requestId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(ct);
        }
    }
}

