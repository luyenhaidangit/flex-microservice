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

            builder.Property(x => x.Code).HasColumnName("CODE").HasColumnType("VARCHAR2(100)").IsRequired();
            builder.Property(x => x.Version).HasColumnName("VERSION").HasColumnType("NUMBER(10)").IsRequired();
            builder.Property(x => x.MakerId).HasColumnName("MAKER_ID").HasColumnType("VARCHAR2(100)").IsRequired();
            builder.Property(x => x.CheckerId).HasColumnName("CHECKER_ID").HasColumnType("VARCHAR2(100)");
            builder.Property(x => x.Status).HasColumnName("STATUS").HasColumnType("VARCHAR2(10)").IsRequired();
            builder.Property(x => x.RequestComment).HasColumnName("REQUEST_COMMENT").HasColumnType("NVARCHAR2(500)");
            builder.Property(x => x.CreatedAt).HasColumnName("CREATED_AT").HasColumnType("TIMESTAMP").HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(x => x.ApprovedAt).HasColumnName("APPROVED_AT").HasColumnType("TIMESTAMP");

            builder.HasIndex(x => new { x.Code, x.Version, x.Status })
                   .HasDatabaseName("IX_WF_DEF_REQ_CODE_VER_STATUS");
        }
    }
}
