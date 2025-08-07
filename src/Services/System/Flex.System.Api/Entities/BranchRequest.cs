using Flex.Contracts.Domains;
using Flex.Shared.SeedWork.Workflow;
using Flex.Shared.SeedWork.Workflow.Constants;

namespace Flex.System.Api.Entities
{
    public class BranchRequest : RequestBase<long>
    {
        public string Action { get; set; } = default!; // CREATE, UPDATE, DELETE
        public long EntityId { get; set; } // ID của entity gốc (0 cho CREATE)
        public string EntityCode { get; set; } = default!; // Code của entity
        public string Status { get; set; } = RequestStatusConstant.Unauthorised;
        public string? CheckerId { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string? Comments { get; set; }
        
        // Data fields (JSON serialized)
        public string RequestData { get; set; } = default!;
        public string? OriginalData { get; set; } // Cho UPDATE/DELETE
    }
}
