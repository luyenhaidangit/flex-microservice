using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Entities.Views;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Contracts.Domains.Interfaces;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
	public interface IUserRequestRepository : IRepositoryBase<UserRequest, long, IdentityDbContext>
	{
		IQueryable<UserRequestView> GetAllUserRequests();
		Task<bool> ExistsPendingByUserNameAsync(string userName, CancellationToken ct = default);
		Task<bool> ExistsPendingByEmailAsync(string email, CancellationToken ct = default);
	}
}


