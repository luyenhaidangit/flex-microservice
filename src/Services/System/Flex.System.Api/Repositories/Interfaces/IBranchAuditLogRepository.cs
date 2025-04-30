using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Entities;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchAuditLogRepository : IRepositoryBase<BranchAuditLog, long, SystemDbContext>
    {
    }
}
