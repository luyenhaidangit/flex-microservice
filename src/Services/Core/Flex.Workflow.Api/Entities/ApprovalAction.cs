using Flex.Contracts.Domains;

namespace Flex.Workflow.Api.Entities
{
    public class ApprovalAction : EntityBase<long>
    {
        public long RequestId { get; set; }
        
        public int Step { get; set; }
        
        public string Action { get; set; } = string.Empty; // APPROVE / REJECT / HOLD / CANCEL
        
        public string ActorId { get; set; } = string.Empty;
        
        public string? Comment { get; set; }
        
        public string? EvidenceUrl { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
