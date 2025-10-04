using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class ApprovalActionConfiguration : IEntityTypeConfiguration<ApprovalAction>
    {
        public void Configure(EntityTypeBuilder<ApprovalAction> builder)
        {
            builder.ToTable("WORKFLOW_ACTIONS");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Step).HasColumnType("NUMBER(10)");

            builder.Property(x => x.Action)
                   .HasColumnType("VARCHAR2(20)")
                   .IsRequired();

            builder.Property(x => x.ActorId)
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.Comment)
                   .HasColumnType("NVARCHAR2(500)");

            builder.Property(x => x.EvidenceUrl)
                   .HasColumnType("VARCHAR2(500)");

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("TIMESTAMP")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.HasIndex(x => new { x.RequestId, x.Step })
                   .HasDatabaseName("IX_WORKFLOW_ACTIONS_REQUEST_STEP");
        }
    }
}
