using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class IdempotencyKeyConfiguration : IEntityTypeConfiguration<IdempotencyKey>
    {
        public void Configure(EntityTypeBuilder<IdempotencyKey> builder)
        {
            builder.ToTable("WF_IDEMPOTENCY_KEYS");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Key).IsUnique();
            builder.Property(x => x.Key).HasMaxLength(200);
            builder.Property(x => x.Fingerprint).HasMaxLength(200);
        }
    }
}

