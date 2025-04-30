using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Entities;
using Flex.System.Api.Persistence;
using Flex.System.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.System.Api.Repositories
{
    public class BranchRequestHeaderRepository
        : RepositoryBase<BranchRequestHeader, long, SystemDbContext>, IBranchRequestHeaderRepository
    {
        public BranchRequestHeaderRepository(SystemDbContext dbContext, IUnitOfWork<SystemDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
        }
    }
}
