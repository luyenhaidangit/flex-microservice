using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Entities.Views;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
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
	}
}

