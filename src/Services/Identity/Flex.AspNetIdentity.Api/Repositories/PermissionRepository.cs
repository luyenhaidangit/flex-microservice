using Flex.AspNetIdentity.Api.Entities;
using Flex.Infrastructure.Common.Repositories;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using Flex.Shared.Authorization;

namespace Flex.AspNetIdentity.Api.Repositories
{
    public class PermissionRepository : RepositoryBase<Permission, long, IdentityDbContext>, IPermissionRepository
    {
        private readonly IdentityDbContext _context;
        public PermissionRepository(IdentityDbContext context, IUnitOfWork<IdentityDbContext> unitOfWork)
            : base(context, unitOfWork)
        {
            _context = context;
        }

        public async Task<List<Permission>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Permissions.AsNoTracking().ToListAsync(ct);
        }

        public async Task<List<string>> GetPermissionCodesOfRoleAsync(long roleId, CancellationToken ct = default)
        {
            return await _context.RoleClaims
                .Where(rc => rc.RoleId == roleId && rc.ClaimType == ClaimTypes.Permission)
                .Select(rc => rc.ClaimValue!)
                .Where(v => v != null)
                .Distinct()
                .ToListAsync(ct);
        }
    }
}
