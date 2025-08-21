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

		public async Task<PagedResult<UserPagingDto>> GetApprovedUsersPagedAsync(GetUsersPagingRequest request, CancellationToken ct = default)
		{
			var keyword = request.Keyword?.Trim().ToLowerInvariant();
			int pageIndex = request.PageIndexValue;
			int pageSize = request.PageSizeValue;

			var query = _context.Set<User>().AsNoTracking()
				.WhereIf(!string.IsNullOrEmpty(keyword),
					u => EF.Functions.Like((u.UserName ?? string.Empty).ToLower(), $"%{keyword}%")
					  || EF.Functions.Like((u.Email ?? string.Empty).ToLower(), $"%{keyword}%")
					  || EF.Functions.Like((u.FullName ?? string.Empty).ToLower(), $"%{keyword}%"))
				.WhereIf(request.BranchId.HasValue, u => u.BranchId == request.BranchId!.Value)
                .WhereIf(request.IsLocked is true, u => u.LockoutEnd.HasValue && u.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow)
				.WhereIf(request.IsLocked is false, u => !u.LockoutEnd.HasValue || u.LockoutEnd.Value.UtcDateTime <= DateTime.UtcNow);

            var total = await query.CountAsync(ct);
			var raw = await query
				.OrderBy(u => u.Id)
				.Skip((pageIndex - 1) * pageSize)
				.Take(pageSize)
				.Select(u => new { u.UserName, u.FullName, u.Email, u.PhoneNumber, u.BranchId, u.LockoutEnd })
				.ToListAsync(ct);

			var items = raw.Select(u => new UserPagingDto
			{
				UserName = u.UserName ?? string.Empty,
				FullName = u.FullName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber,
				BranchId = u.BranchId,
				IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow,
				IsActive = true
			}).ToList();

			return PagedResult<UserPagingDto>.Create(pageIndex, pageSize, total, items);
		}

		public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
		{
			return await _context.Set<User>().AsNoTracking().AnyAsync(u => u.UserName == userName, ct);
		}
	}
}


