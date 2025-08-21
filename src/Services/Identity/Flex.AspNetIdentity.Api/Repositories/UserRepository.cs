using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Infrastructure.EF;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Flex.AspNetIdentity.Api.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly IdentityDbContext _context;

		public UserRepository(IdentityDbContext dbContext)
		{
			_context = dbContext;
		}

		public async Task<PagedResult<UserPagingDto>> GetApprovedUsersPagedAsync(GetUsersPagingRequest request, CancellationToken ct = default)
		{
			var keyword = request?.Keyword?.Trim().ToLower();
			int pageIndex = Math.Max(1, request.PageIndex ?? 1);
			int pageSize = Math.Max(1, request.PageSize ?? 10);

			var query = _context.Set<User>()
				.WhereIf(!string.IsNullOrEmpty(keyword),
					u => EF.Functions.Like((u.UserName ?? string.Empty).ToLower(), $"%{keyword}%")
					  || EF.Functions.Like((u.Email ?? string.Empty).ToLower(), $"%{keyword}%")
					  || EF.Functions.Like((u.FullName ?? string.Empty).ToLower(), $"%{keyword}%"))
				.WhereIf(request?.BranchId != null, u => u.BranchId == request!.BranchId);

			if (request?.IsLocked != null)
			{
				var isLocked = request.IsLocked.Value;
				query = query.Where(u => (u.LockoutEnd.HasValue && u.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow) == isLocked);
			}

			var projected = query.Select(u => new UserPagingDto
			{
				UserName = u.UserName ?? string.Empty,
				FullName = u.FullName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber,
				BranchId = u.BranchId,
				IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow,
				IsActive = true
			}).AsNoTracking();

			return await projected.ToPagedResultAsync(request);
		}

		public async Task<User?> GetByUserNameAsync(string userName, bool asNoTracking = true, CancellationToken ct = default)
		{
			var query = _context.Set<User>().Where(u => u.UserName == userName);
			if (asNoTracking)
			{
				query = query.AsNoTracking();
			}
			return await query.FirstOrDefaultAsync(ct);
		}

		public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
		{
			return await _context.Set<User>().AsNoTracking().AnyAsync(u => u.UserName == userName, ct);
		}
	}
}


