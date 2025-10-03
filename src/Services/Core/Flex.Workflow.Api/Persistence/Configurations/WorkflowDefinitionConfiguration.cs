using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
    {
        public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
        {
            builder.ToTable("WF_DEFINITIONS");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).HasMaxLength(100);
            builder.Property(x => x.Name).HasMaxLength(200);
            builder.Property(x => x.Description).HasMaxLength(500);
            builder.Property(x => x.Steps).HasColumnType("clob");
            builder.Property(x => x.Policy).HasColumnType("clob");
            builder.Property(x => x.UpdatedBy).HasMaxLength(100);
            builder.HasIndex(x => new { x.Code, x.Version }).IsUnique();
            builder.HasIndex(x => x.IsActive);
        }
    }
}
