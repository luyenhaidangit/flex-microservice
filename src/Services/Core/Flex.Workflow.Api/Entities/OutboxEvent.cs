using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Workflow.Api.Entities
{
    public class OutboxEvent : EntityBase<long>
    {
        [Required]
        [Column("AGGREGATE", TypeName = "VARCHAR2(100)")]
        public string Aggregate { get; set; } = string.Empty; // workflow.request

        [Required]
        [Column("AGGREGATE_ID", TypeName = "VARCHAR2(100)")]
        public string AggregateId { get; set; } = string.Empty; // request id

        [Required]
        [Column("EVENT_TYPE", TypeName = "VARCHAR2(100)")]
        public string EventType { get; set; } = string.Empty; // request.created, request.approved

        [Required]
        [Column("PAYLOAD", TypeName = "CLOB")]
        public string Payload { get; set; } = string.Empty;

        [Required]
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("SENT_AT")]
        public DateTime? SentAt { get; set; }
    }
}

