using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowDefinitionPublishRequestConfiguration : IEntityTypeConfiguration<WorkflowDefinitionPublishRequest>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinitionPublishRequest> builder)
        {
            builder.ToTable("WORKFLOW_DEFINITION_REQUESTS");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("ID")
                   .HasColumnType("NUMBER(19)")
                   .UseIdentityColumn();

            builder.Property(x => x.Code).HasColumnType("VARCHAR2(100)").IsRequired();
            builder.Property(x => x.Version).HasColumnType("NUMBER(10)").IsRequired();
            builder.Property(x => x.MakerId).HasColumnType("VARCHAR2(100)").IsRequired();
            builder.Property(x => x.CheckerId).HasColumnType("VARCHAR2(100)");
            builder.Property(x => x.Status).HasColumnType("VARCHAR2(10)").IsRequired();
            builder.Property(x => x.RequestComment).HasColumnType("NVARCHAR2(500)");
            builder.Property(x => x.CreatedAt).HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.ApprovedAt).HasColumnType("TIMESTAMP");

            builder.HasIndex(x => new { x.Code, x.Version, x.Status })
                   .HasDatabaseName("IX_WF_DEF_REQ_CODE_VER_STATUS");
        }
    }
}

