using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
    {
        public void Configure(EntityTypeBuilder<OutboxEvent> builder)
        {
            builder.ToTable("WF_OUTBOX_EVENTS");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Aggregate).HasMaxLength(100);
            builder.Property(x => x.AggregateId).HasMaxLength(100);
            builder.Property(x => x.EventType).HasMaxLength(100);
            builder.Property(x => x.Payload).HasColumnType("clob");
            builder.HasIndex(x => new { x.Aggregate, x.AggregateId, x.EventType });
            builder.HasIndex(x => x.SentAt);
        }
    }
}
