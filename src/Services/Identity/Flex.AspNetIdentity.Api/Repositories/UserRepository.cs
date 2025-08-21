using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.EF;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Flex.AspNetIdentity.Api.Repositories
{
	public class UserRepository : RepositoryBase<User, long, IdentityDbContext> ,IUserRepository
	{
		private readonly IdentityDbContext _context;

		public UserRepository(IdentityDbContext context, IUnitOfWork<IdentityDbContext> unitOfWork) : base(context, unitOfWork)
		{
			_context = context;
		}

		public async Task<PagedResult<UserPagingDto>> GetUsersPagedAsync(GetUsersPagingRequest request, CancellationToken ct = default)
		{
			var keyword = request.Keyword?.Trim().ToLowerInvariant();

			var baseQuery = _context.Set<User>().AsNoTracking()
				.WhereIf(!string.IsNullOrEmpty(keyword),
					u => EF.Functions.Like((u.UserName ?? string.Empty).ToLower(), $"%{keyword}%")
					  || EF.Functions.Like((u.Email ?? string.Empty).ToLower(), $"%{keyword}%")
					  || EF.Functions.Like((u.FullName ?? string.Empty).ToLower(), $"%{keyword}%"))
				.WhereIf(request.BranchId.HasValue, u => u.BranchId == request.BranchId!.Value)
				.WhereIf(request.IsLocked is true, u => u.LockoutEnd.HasValue && u.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow)
				.WhereIf(request.IsLocked is false, u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value.UtcDateTime <= DateTime.UtcNow);

			var dtoQuery = baseQuery.Select(u => new UserPagingDto
			{
				UserName = u.UserName ?? string.Empty,
				FullName = u.FullName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber,
				BranchId = u.BranchId,
				IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow,
				IsActive = true
			});

			return await dtoQuery.ToPagedResultAsync(request);
		}

		public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
		{
			return await _context.Set<User>().AsNoTracking().AnyAsync(u => u.UserName == userName, ct);
		}
	}
}


