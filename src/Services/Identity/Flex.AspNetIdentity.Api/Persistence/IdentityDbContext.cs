using Flex.AspNetIdentity.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Flex.AspNetIdentity.Api.Entities.Views;

namespace Flex.AspNetIdentity.Api.Persistence
{
    public class IdentityDbContext : DbContext
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
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<RoleRequest> RoleRequests { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<BranchCache> BranchCaches { get; set; }
        #endregion

        #region View
        public DbSet<ProposedBranch> ProposedBranchs { get; set; }
        public DbSet<UserRequestView> UserRequestViews { get; set; }
        #endregion
    }
}
