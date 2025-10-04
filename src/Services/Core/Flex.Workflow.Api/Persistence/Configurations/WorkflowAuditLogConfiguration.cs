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

            builder.Property(x => x.Id)
                   .HasColumnName("ID")
                   .HasColumnType("NUMBER(19)")
                   .UseIdentityColumn();
            builder.Property(x => x.Event)
                   .HasColumnName("EVENT")
                   .HasColumnType("VARCHAR2(50)")
                   .IsRequired();

            builder.Property(x => x.ActorId)
                   .HasColumnName("ACTOR_ID")
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.Metadata)
                   .HasColumnName("METADATA")
                   .HasColumnType("CLOB");

            builder.Property(x => x.PrevHash)
                   .HasColumnName("PREV_HASH")
                   .HasColumnType("VARCHAR2(128)");

            builder.Property(x => x.CurrHash)
                   .HasColumnName("CURR_HASH")
                   .HasColumnType("VARCHAR2(128)")
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasColumnName("CREATED_AT")
                   .HasColumnType("TIMESTAMP")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.RequestId)
                   .HasColumnName("REQUEST_ID");
            builder.HasIndex(x => x.RequestId)
                   .HasDatabaseName("IX_WORKFLOW_AUDIT_LOGS_REQUEST");
        }
    }
}
