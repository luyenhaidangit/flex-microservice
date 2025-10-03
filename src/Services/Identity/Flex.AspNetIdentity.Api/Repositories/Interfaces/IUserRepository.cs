using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Contracts.Domains.Interfaces;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
	public interface IUserRepository : IRepositoryBase<User, long, IdentityDbContext>
    {
		Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
		Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
		Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
    }
}


