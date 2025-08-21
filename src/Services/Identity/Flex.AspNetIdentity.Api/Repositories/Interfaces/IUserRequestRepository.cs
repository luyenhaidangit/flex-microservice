using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Entities.Views;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.Contracts.Domains.Interfaces;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
	public interface IUserRequestRepository : IRepositoryBase<UserRequest, long, IdentityDbContext>
	{
		IQueryable<ProposedUser> GetBranchCombinedQuery();
	}
}


