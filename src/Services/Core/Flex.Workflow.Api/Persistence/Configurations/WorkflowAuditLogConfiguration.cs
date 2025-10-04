using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowAuditLogConfiguration : IEntityTypeConfiguration<WorkflowAuditLog>
    {
        public void Configure(EntityTypeBuilder<WorkflowAuditLog> builder)
        {
            builder.ToTable("WORKFLOW_AUDIT_LOGS");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Event)
                   .HasColumnType("VARCHAR2(50)")
                   .IsRequired();

            builder.Property(x => x.ActorId)
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.Metadata)
                   .HasColumnType("CLOB");

            builder.Property(x => x.PrevHash)
                   .HasColumnType("VARCHAR2(128)");

            builder.Property(x => x.CurrHash)
                   .HasColumnType("VARCHAR2(128)")
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("TIMESTAMP")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.HasIndex(x => x.RequestId)
                   .HasDatabaseName("IX_WORKFLOW_AUDIT_LOGS_REQUEST");
        }
    }
}
