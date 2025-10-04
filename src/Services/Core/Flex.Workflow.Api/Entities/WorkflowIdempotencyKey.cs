using Flex.Contracts.Domains;

namespace Flex.Workflow.Api.Entities
{
    public class WorkflowIdempotencyKey : EntityBase<long>
    {
        public string Key { get; set; } = string.Empty;
        
        public string Fingerprint { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
