using Flex.Contracts.Domains;

namespace Flex.Workflow.Api.Entities
{
    public class OutboxEvent : EntityBase<long>
    {
        public string Aggregate { get; set; } = string.Empty; // workflow.request
        
        public string AggregateId { get; set; } = string.Empty; // request id
        
        public string EventType { get; set; } = string.Empty; // request.created, request.approved
        
        public string Payload { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? SentAt { get; set; }
    }
}
