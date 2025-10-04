using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flex.Workflow.Api.Persistence
{
    public class WorkflowDbContext : DbContext
    {
        public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
        {
        }

        public DbSet<WorkflowDefinition> WorkflowDefinitions => Set<WorkflowDefinition>();
        public DbSet<WorkflowRequest> WorkflowRequests => Set<WorkflowRequest>();
        public DbSet<WorkflowAction> WorkflowActions => Set<WorkflowAction>();
        public DbSet<WorkflowAuditLog> WorkflowAuditLogs => Set<WorkflowAuditLog>();
        public DbSet<WorkflowOutboxEvent> WorkflowOutboxEvents => Set<WorkflowOutboxEvent>();
        public DbSet<WorkflowIdempotencyKey> WorkflowIdempotencyKeys => Set<WorkflowIdempotencyKey>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowDbContext).Assembly);
        }
    }
}
