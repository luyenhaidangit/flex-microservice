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

        public async Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
		{
			return await _context.Set<User>().AsNoTracking().AnyAsync(u => u.UserName == userName, ct);
		}
	}
}


