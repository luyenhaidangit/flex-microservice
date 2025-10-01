using Flex.Infrastructure.EntityFrameworkCore;
using Flex.Infrastructure.EntityFrameworkCore;
using Flex.Notification.Api.Entities;
using Flex.Notification.Api.Entities.Views;
using Microsoft.EntityFrameworkCore;

namespace Flex.Notification.Api.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {
        }

        #region DbSet
        // Entities
        public DbSet<NotificationLog> NotificationLogs { get; set; }
        public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

        // Views
        public DbSet<NotificationTemplateRequestView> NotificationTemplateRequestViews { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
            modelBuilder.ApplyApprovalRequests(AssemblyReference.Assembly);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.DefaultTypeMapping<string>(
                b => b.IsUnicode(false));
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
