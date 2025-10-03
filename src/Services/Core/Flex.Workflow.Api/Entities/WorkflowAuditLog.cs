using Flex.Contracts.Domains;

namespace Flex.Workflow.Api.Entities
{
    public class WorkflowAuditLog : EntityBase<long>
    {
        public long RequestId { get; set; }
        
        public string Event { get; set; } = string.Empty; // created, submitted, approved, rejected, escalated, expired
        
        public string ActorId { get; set; } = string.Empty;
        
        public string? Metadata { get; set; }
        
        public string? PrevHash { get; set; }
        
        public string CurrHash { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
