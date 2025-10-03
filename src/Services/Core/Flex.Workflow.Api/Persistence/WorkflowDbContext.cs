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
        public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>();
        public DbSet<ApprovalAction> ApprovalActions => Set<ApprovalAction>();
        public DbSet<WorkflowAuditLog> WorkflowAuditLogs => Set<WorkflowAuditLog>();
        public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();
        public DbSet<IdempotencyKey> IdempotencyKeys => Set<IdempotencyKey>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowDbContext).Assembly);
        }
    }
}

