using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<Flex.Shared.SeedWork.PagedResult<UserApprovedListItemDto>> GetApprovedUsersPagedAsync(GetUsersPagingRequest request, CancellationToken ct = default);
		Task<User?> GetByUserNameAsync(string userName, bool asNoTracking = true, CancellationToken ct = default);
		Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
	}
}


