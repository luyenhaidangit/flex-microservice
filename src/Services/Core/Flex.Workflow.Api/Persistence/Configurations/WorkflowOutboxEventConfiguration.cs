using Flex.Workflow.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.Workflow.Api.Persistence.Configurations
{
    public class WorkflowOutboxEventConfiguration : IEntityTypeConfiguration<WorkflowOutboxEvent>
    {
        public void Configure(EntityTypeBuilder<WorkflowOutboxEvent> builder)
        {
            builder.ToTable("WORKFLOW_OUTBOX_EVENTS");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .HasColumnName("ID")
                   .HasColumnType("NUMBER(19)")
                   .UseIdentityColumn();
            builder.Property(x => x.Aggregate)
                   .HasColumnName("AGGREGATE")
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.AggregateId)
                   .HasColumnName("AGGREGATE_ID")
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.EventType)
                   .HasColumnName("EVENT_TYPE")
                   .HasColumnType("VARCHAR2(100)")
                   .IsRequired();

            builder.Property(x => x.Payload)
                   .HasColumnName("PAYLOAD")
                   .HasColumnType("CLOB")
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasColumnName("CREATED_AT")
                   .HasColumnType("TIMESTAMP")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.SentAt)
                   .HasColumnName("SENT_AT")
                   .HasColumnType("TIMESTAMP");
            builder.HasIndex(x => new { x.Aggregate, x.AggregateId, x.EventType })
                   .HasDatabaseName("IX_WORKFLOW_OUTBOX_AGGREGATE_EVENT");

            builder.HasIndex(x => x.SentAt)
                   .HasDatabaseName("IX_WORKFLOW_OUTBOX_SENT_AT");
        }
    }
}
