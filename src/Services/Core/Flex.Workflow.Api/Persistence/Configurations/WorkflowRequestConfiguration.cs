using Flex.Infrastructure.EntityFrameworkCore.Configurations;
using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowRequestConfiguration : RequestBaseConfiguration<WorkflowRequest, string>, IEntityTypeConfiguration<WorkflowRequest>
    {
        public override void Configure(EntityTypeBuilder<WorkflowRequest> builder)
        {
            base.Configure(builder);

            builder.ToTable("WORKFLOW_REQUESTS");

            builder.Property(x => x.Id)
                   .UseIdentityColumn()
                   .HasColumnType("NUMBER(19)")
                   .HasColumnName("ID");

            builder.Property(x => x.EntityId)
                   .HasColumnName("ENTITY_KEY")
                   .HasColumnType("VARCHAR2(200)")
                   .IsRequired();

            builder.Property(x => x.Domain)
                   .HasColumnName("DOMAIN")
                   .HasColumnType("VARCHAR2(50)")
                   .IsRequired();

            builder.Property(x => x.WorkflowCode)
                   .HasColumnName("WORKFLOW_CODE")
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.BusinessId)
                   .HasColumnName("BUSINESS_ID")
                   .HasColumnType("VARCHAR2(200)");

            builder.Property(x => x.CorrelationId)
                   .HasColumnName("CORRELATION_ID")
                   .HasColumnType("VARCHAR2(100)");

            builder.HasIndex(x => new { x.Domain, x.WorkflowCode, x.Action, x.Status })
                   .HasDatabaseName("IX_WORKFLOW_REQ_DOMAIN_CODE_ACTION_STATUS");

            builder.HasIndex(x => x.BusinessId)
                   .HasDatabaseName("IX_WORKFLOW_REQ_BUSINESS_ID");
        }
    }
}
