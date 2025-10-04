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
                   .HasColumnName("CODE")
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.Name)
                   .HasColumnName("NAME")
                   .HasColumnType("NVARCHAR2(200)")
                   .IsRequired();

            builder.Property(x => x.Description)
                   .HasColumnName("DESCRIPTION")
                   .HasColumnType("NVARCHAR2(500)");

            builder.Property(x => x.Steps)
                   .HasColumnName("STEPS")
                   .HasColumnType("CLOB");

            builder.Property(x => x.Policy)
                   .HasColumnName("POLICY")
                   .HasColumnType("CLOB");

            builder.Property(x => x.UpdatedBy)
                   .HasColumnName("UPDATED_BY")
                   .HasColumnType("VARCHAR2(100)");

            builder.Property(x => x.IsActive)
                   .HasColumnName("IS_ACTIVE")
                   .HasColumnType("NUMBER(1)")
                   .HasConversion<int>();

            builder.Property(x => x.State)
                   .HasColumnName("STATE")
                   .HasColumnType("VARCHAR2(20)")
                   .IsRequired();

            builder.Property(x => x.Version)
                   .HasColumnName("VERSION")
                   .HasColumnType("NUMBER(10)");

            builder.Property(x => x.UpdatedAt)
                   .HasColumnName("UPDATED_AT")
                   .HasColumnType("TIMESTAMP");
            builder.HasIndex(x => new { x.Code, x.Version })
                   .IsUnique()
                   .HasDatabaseName("UX_WORKFLOW_DEFINITIONS_CODE_VER");

            builder.HasIndex(x => x.IsActive)
                   .HasDatabaseName("IX_WORKFLOW_DEFINITIONS_IS_ACTIVE");

            builder.HasIndex(x => new { x.Code, x.State })
                   .HasDatabaseName("IX_WORKFLOW_DEFINITIONS_CODE_STATE");
        }
    }
}
