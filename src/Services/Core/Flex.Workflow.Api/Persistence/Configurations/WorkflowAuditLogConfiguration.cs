using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowAuditLogConfiguration : IEntityTypeConfiguration<WorkflowAuditLog>
    {
        public void Configure(EntityTypeBuilder<WorkflowAuditLog> builder)
        {
            builder.ToTable("WF_AUDIT_LOGS");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Event).HasMaxLength(50);
            builder.Property(x => x.ActorId).HasMaxLength(100);
            builder.Property(x => x.Metadata).HasColumnType("clob");
            builder.Property(x => x.PrevHash).HasMaxLength(128);
            builder.Property(x => x.CurrHash).HasMaxLength(128);
            builder.HasIndex(x => x.RequestId);
        }
    }
}
