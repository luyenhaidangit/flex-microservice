using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Entities;
using Flex.System.Api.Persistence;
using Flex.System.Api.Repositories.Interfaces;

namespace Flex.System.Api.Repositories
{
    public class BranchAuditLogRepository
        : RepositoryBase<BranchAuditLog, long, SystemDbContext>, IBranchAuditLogRepository
    {
        public BranchAuditLogRepository(SystemDbContext dbContext, IUnitOfWork<SystemDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }
    }
}
