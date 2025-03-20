using Flex.Contracts.Domains.Interfaces;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Repositories.Interfaces
{
    public interface IDepartmentRepository : IRepositoryBase<Entities.Department, long, SystemDbContext>
    {
    }
}
