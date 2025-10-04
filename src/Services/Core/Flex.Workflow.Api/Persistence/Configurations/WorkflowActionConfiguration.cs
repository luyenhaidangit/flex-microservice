using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowActionConfiguration : IEntityTypeConfiguration<WorkflowAction>
    {
        public void Configure(EntityTypeBuilder<WorkflowAction> builder)
        {
            builder.ToTable("WORKFLOW_ACTIONS");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("ID")
                   .HasColumnType("NUMBER(19)")
                   .UseIdentityColumn();

            builder.Property(x => x.Step)
                   .HasColumnName("STEP")
                   .HasColumnType("NUMBER(10)");

            builder.Property(x => x.Action)
                   .HasColumnName("ACTION")
                   .HasColumnType("VARCHAR2(20)")
                   .IsRequired();

            builder.Property(x => x.ActorId)
                   .HasColumnName("ACTOR_ID")
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.Comment)
                   .HasColumnName("ACTION_COMMENT")
                   .HasColumnType("NVARCHAR2(500)");

            builder.Property(x => x.EvidenceUrl)
                   .HasColumnName("EVIDENCE_URL")
                   .HasColumnType("VARCHAR2(500)");

            builder.Property(x => x.CreatedAt)
                   .HasColumnName("CREATED_AT")
                   .HasColumnType("TIMESTAMP")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.RequestId)
                   .HasColumnName("REQUEST_ID");
            builder.HasIndex(x => new { x.RequestId, x.Step })
                   .HasDatabaseName("IX_WORKFLOW_ACTIONS_REQUEST_STEP");
        }
    }
}
