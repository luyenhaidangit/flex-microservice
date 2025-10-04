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

            builder.ToTable("WORKFLOW_REQUESTS");

            builder.Property(x => x.Domain).HasMaxLength(50);
            builder.Property(x => x.WorkflowCode).HasMaxLength(100);
            builder.Property(x => x.BusinessId).HasMaxLength(200);
            builder.Property(x => x.CorrelationId).HasMaxLength(100);
            builder.Property(x => x.Domain).HasColumnType("VARCHAR2(50)").IsRequired();
            builder.Property(x => x.WorkflowCode).HasColumnType("VARCHAR2(100)").IsRequired();
            builder.Property(x => x.BusinessId).HasColumnType("VARCHAR2(200)");
            builder.Property(x => x.CorrelationId).HasColumnType("VARCHAR2(100)");

            builder.HasIndex(x => new { x.Domain, x.WorkflowCode, x.Action, x.Status })
                   .HasDatabaseName("IX_WORKFLOW_REQ_DOMAIN_CODE_ACTION_STATUS");

            builder.HasIndex(x => x.BusinessId)
                   .HasDatabaseName("IX_WORKFLOW_REQ_BUSINESS_ID");
        }
    }
}
