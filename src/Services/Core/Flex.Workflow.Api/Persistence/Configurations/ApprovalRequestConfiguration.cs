using Flex.Infrastructure.EntityFrameworkCore.Configurations;
using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class ApprovalRequestConfiguration : RequestBaseConfiguration<ApprovalRequest, long>, IEntityTypeConfiguration<ApprovalRequest>
    {
        public override void Configure(EntityTypeBuilder<ApprovalRequest> builder)
        {
            base.Configure(builder);

            builder.ToTable("WF_REQUESTS");

            builder.Property(x => x.Domain).HasMaxLength(50);
            builder.Property(x => x.WorkflowCode).HasMaxLength(100);
            builder.Property(x => x.BusinessId).HasMaxLength(200);
            builder.Property(x => x.CorrelationId).HasMaxLength(100);

            builder.HasIndex(x => new { x.Domain, x.WorkflowCode, x.Action, x.Status });
            builder.HasIndex(x => x.BusinessId);
        }
    }
}

