using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Entities.Views;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Shared.SeedWork.Workflow.Constants;
using Microsoft.EntityFrameworkCore;

namespace Flex.AspNetIdentity.Api.Repositories
{
	public class UserRequestRepository : RepositoryBase<UserRequest, long, IdentityDbContext>, IUserRequestRepository
	{
		private readonly IdentityDbContext _context;

		public UserRequestRepository(IdentityDbContext dbContext, IUnitOfWork<IdentityDbContext> unitOfWork)
			: base(dbContext, unitOfWork)
		{
			_context = dbContext;
		}

		public IQueryable<UserRequestView> GetAllUserRequests()
		{
			return _context.Set<UserRequestView>().AsNoTracking();
		}

		public async Task<bool> ExistsPendingByUserNameAsync(string userName, long? excludeRequestId = null, CancellationToken ct = default)
		{
			var query = GetAllUserRequests()
				.Where(ur => EF.Functions.Like(ur.UserName.ToLower(), userName.ToLower()) 
					&& ur.Status == RequestStatusConstant.Unauthorised);

			// Exclude current request if specified
			if (excludeRequestId.HasValue)
			{
				query = query.Where(ur => ur.RequestId != excludeRequestId.Value);
			}

			var count = await query.CountAsync(ct);
			return count > 0;
		}

		public async Task<bool> ExistsPendingByEmailAsync(string email, long? excludeRequestId = null, CancellationToken ct = default)
		{
			var query = GetAllUserRequests()
				.Where(ur => EF.Functions.Like(ur.Email.ToLower(), email.ToLower()) 
					&& ur.Status == RequestStatusConstant.Unauthorised);

			// Exclude current request if specified
			if (excludeRequestId.HasValue)
			{
				query = query.Where(ur => ur.RequestId != excludeRequestId.Value);
			}

			var count = await query.CountAsync(ct);
			return count > 0;
		}
	}
}

