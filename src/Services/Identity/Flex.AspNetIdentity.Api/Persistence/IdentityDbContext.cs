using Flex.AspNetIdentity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Flex.AspNetIdentity.Api.Entities.Views;

namespace Flex.AspNetIdentity.Api.Persistence
{
    public class IdentityDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.DefaultTypeMapping<string>(
                b => b.IsUnicode(false));
        }

        #region DbSet
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<RoleRequest> RoleRequests { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        #endregion

        #region View
        public DbSet<ProposedBranch> ProposedBranchs { get; set; }
        #endregion
    }
}
