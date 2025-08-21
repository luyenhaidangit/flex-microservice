using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Contracts.Domains.Interfaces;
using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
	public interface IUserRepository : IRepositoryBase<User, long, IdentityDbContext>
    {
		Task<PagedResult<UserPagingDto>> GetApprovedUsersPagedAsync(GetUsersPagingRequest request, CancellationToken ct = default);
		Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
	}
}


