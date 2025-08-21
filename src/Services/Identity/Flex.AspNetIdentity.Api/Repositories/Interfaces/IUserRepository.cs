using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<PagedResult<UserListItemDto>> GetApprovedUsersPagedAsync(GetUsersPagingRequest request, CancellationToken ct = default);
		Task<User?> GetByUserNameAsync(string userName, bool asNoTracking = true, CancellationToken ct = default);
		Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
	}
}


