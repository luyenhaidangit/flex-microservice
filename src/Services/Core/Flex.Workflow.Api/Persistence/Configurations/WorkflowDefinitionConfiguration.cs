using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
        {
            builder.ToTable("WORKFLOW_DEFINITIONS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("ID")
                   .HasColumnType("NUMBER(19)")
                   .UseIdentityColumn();

            builder.Property(x => x.Code)
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.Name)
                   .HasColumnType("NVARCHAR2(200)")
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasColumnType("NVARCHAR2(500)");

            builder.Property(x => x.Steps)
                   .HasColumnType("CLOB");

            builder.Property(x => x.Policy)
                   .HasColumnType("CLOB");

            builder.Property(x => x.UpdatedBy)
                   .HasColumnType("VARCHAR2(100)");

            builder.Property(x => x.IsActive)
                   .HasColumnType("NUMBER(1)")
                   .HasConversion<int>();

            builder.Property(x => x.Version)
                   .HasColumnType("NUMBER(10)");
            builder.HasIndex(x => new { x.Code, x.Version })
                   .IsUnique()
                   .HasDatabaseName("UX_WORKFLOW_DEFINITIONS_CODE_VER");

            builder.HasIndex(x => x.IsActive)
                   .HasDatabaseName("IX_WORKFLOW_DEFINITIONS_IS_ACTIVE");
        }
    }
}
