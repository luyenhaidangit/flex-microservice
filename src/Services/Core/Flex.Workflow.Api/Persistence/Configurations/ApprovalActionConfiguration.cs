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
            builder.HasIndex(x => new { x.RequestId, x.Step });
        }
    }
}

