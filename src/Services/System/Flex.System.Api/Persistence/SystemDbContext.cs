using Flex.Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.System.Api.Persistence
{
    public class SystemDbContext : DbContext
    {
        public SystemDbContext(DbContextOptions<SystemDbContext> options) : base(options)
        {
        }

        #region DbSet
        public DbSet<Entities.Config> Configs { get; set; }
        public DbSet<Entities.Department> Departments { get; set; }
        public DbSet<Entities.DepartmentRequest> DepartmentRequests { get; set; }
        public DbSet<Entities.Branch> Branches { get; set; }
        public DbSet<Entities.BranchRequest> BranchRequests { get; set; }
        public DbSet<Entities.Views.ProposedBranch> ProposedBranches { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //var modified = ChangeTracker.Entries()
            //    .Where(e => e.State == EntityState.Modified ||
            //                e.State == EntityState.Added ||
            //                e.State == EntityState.Deleted);

            //foreach (var item in modified)
            //{
            //    switch (item.State)
            //    {
            //        case EntityState.Added:
            //            if (item.Entity is IDateTracking addedEntity)
            //            {
            //                addedEntity.CreatedDate = DateTime.UtcNow;
            //            }
            //            break;

            //        case EntityState.Modified:
            //            Entry(item.Entity).Property("Id").IsModified = false;
            //            if (item.Entity is IDateTracking modifiedEntity)
            //            {
            //                modifiedEntity.LastModifiedDate = DateTime.UtcNow;
            //                item.State = EntityState.Modified;
            //            }
            //            break;
            //    }
            //}

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
