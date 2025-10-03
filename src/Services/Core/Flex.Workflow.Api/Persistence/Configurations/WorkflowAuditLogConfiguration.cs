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
            builder.HasIndex(x => x.RequestId);
        }
    }
}

