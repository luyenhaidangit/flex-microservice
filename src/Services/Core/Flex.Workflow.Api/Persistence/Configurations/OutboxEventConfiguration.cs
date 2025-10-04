using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
    {
        public void Configure(EntityTypeBuilder<OutboxEvent> builder)
        {
            builder.ToTable("WORKFLOW_OUTBOX_EVENTS");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("ID")
                   .HasColumnType("NUMBER(19)")
                   .UseIdentityColumn();
            builder.Property(x => x.Aggregate)
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.AggregateId)
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.EventType)
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.Payload)
                   .HasColumnType("CLOB")
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("TIMESTAMP")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.SentAt)
                   .HasColumnType("TIMESTAMP");
            builder.HasIndex(x => new { x.Aggregate, x.AggregateId, x.EventType })
                   .HasDatabaseName("IX_WORKFLOW_OUTBOX_AGGREGATE_EVENT");

            builder.HasIndex(x => x.SentAt)
                   .HasDatabaseName("IX_WORKFLOW_OUTBOX_SENT_AT");
        }
    }
}
