using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
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
			var count = await _context.Users.AsNoTracking()
				.Where(u => u.UserName!.ToLower() == userName.ToLower())
				.CountAsync(ct);
			return count > 0;
		}

		public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
		{
			var count = await _context.Users.AsNoTracking()
				.Where(u => u.Email!.ToLower() == email.ToLower())
				.CountAsync(ct);
			return count > 0;
		}
	}
}


