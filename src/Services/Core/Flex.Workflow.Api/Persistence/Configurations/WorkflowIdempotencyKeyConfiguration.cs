using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowIdempotencyKeyConfiguration : IEntityTypeConfiguration<WorkflowIdempotencyKey>
    {
        public void Configure(EntityTypeBuilder<WorkflowIdempotencyKey> builder)
        {
            builder.ToTable("WORKFLOW_IDEMPOTENCY_KEYS");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("ID")
                   .HasColumnType("NUMBER(19)")
                   .UseIdentityColumn();
            builder.HasIndex(x => x.Key)
                   .IsUnique()
                   .HasDatabaseName("UX_WORKFLOW_IDEMPOTENCY_KEYS_KEY");
            builder.Property(x => x.Key)
                   .HasColumnType("VARCHAR2(200)")
                   .IsRequired();

            builder.Property(x => x.Fingerprint)
                   .HasColumnType("VARCHAR2(200)")
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("TIMESTAMP")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
