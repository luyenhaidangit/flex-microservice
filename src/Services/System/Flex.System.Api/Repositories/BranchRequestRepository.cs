using Microsoft.EntityFrameworkCore;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.System.Api.Entities;
using Flex.System.Api.Models;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Flex.System.Api.Persistence;
using Flex.Infrastructure.EF;
using Flex.System.Api.Models.Branch;

namespace Flex.System.Api.Repositories
{
    public class BranchRequestRepository : RepositoryBase<BranchRequest, long, SystemDbContext>, IBranchRequestRepository
    {
        private readonly SystemDbContext _context;
        public BranchRequestRepository(SystemDbContext dbContext, IUnitOfWork<SystemDbContext> unitOfWork)
            : base(dbContext, unitOfWork)
        {
            _context = dbContext;
        }

        public async Task<BranchRequest?> GetPendingByIdAsync(long requestId)
        {
            return await _context.BranchRequests
                .FirstOrDefaultAsync(x => x.Id == requestId && x.Status == RequestStatusConstant.Unauthorised);
        }
    }
}
