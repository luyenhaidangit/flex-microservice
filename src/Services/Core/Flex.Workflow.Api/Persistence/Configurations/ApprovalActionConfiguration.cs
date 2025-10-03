using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class ApprovalActionConfiguration : IEntityTypeConfiguration<ApprovalAction>
    {
        public void Configure(EntityTypeBuilder<ApprovalAction> builder)
        {
            builder.ToTable("WF_ACTIONS");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Step).HasColumnType("number(10)");
            builder.Property(x => x.Action).HasMaxLength(20);
            builder.Property(x => x.ActorId).HasMaxLength(100);
            builder.Property(x => x.Comment).HasMaxLength(500);
            builder.Property(x => x.EvidenceUrl).HasMaxLength(500);
            builder.HasIndex(x => new { x.RequestId, x.Step });
        }
    }
}
