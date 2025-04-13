using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IBranchRepository : IRepositoryBase<Entities.Branch, long, SystemDbContext>
    {
        Task<bool> HasAccounts(long branchId);
        Task<bool> HasTransactions(long branchId);
        Task<bool> HasUsers(long branchId);
    }
}
