using Flex.Infrastructure.Workflow.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flex.Infrastructure.Workflow.Persistence;

public class WorkflowDbContext : DbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
    {
    }

    public DbSet<WorkflowRequest> Requests => Set<WorkflowRequest>();
    public DbSet<WorkflowStep> Steps => Set<WorkflowStep>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowRequest>(b =>
        {
            b.ToTable("WORKFLOW_REQUESTS");
            b.HasKey(x => x.Id);
            b.Property(x => x.WorkflowType).HasMaxLength(100).IsRequired();
            b.Property(x => x.EntityId).HasMaxLength(100).IsRequired();
            b.Property(x => x.Status).HasMaxLength(20).HasConversion<string>();
            b.Property(x => x.RequestedBy).HasMaxLength(100).IsRequired();
            b.Property(x => x.Comment).HasMaxLength(500);
            b.Property(x => x.CorrelationId).HasMaxLength(100);
            b.HasIndex(x => new { x.WorkflowType, x.Status, x.RequestedAt });
            b.HasIndex(x => x.CorrelationId).IsUnique(false);
        });

        modelBuilder.Entity<WorkflowStep>(b =>
        {
            b.ToTable("WORKFLOW_STEPS");
            b.HasKey(x => x.Id);
            b.Property(x => x.ApproverRole).HasMaxLength(100);
            b.Property(x => x.ApproverUser).HasMaxLength(100);
            b.Property(x => x.Decision).HasMaxLength(20).HasConversion<string>();
            b.Property(x => x.Comment).HasMaxLength(500);
            b.HasIndex(x => new { x.RequestId, x.Level, x.Order });
        });
    }
}

