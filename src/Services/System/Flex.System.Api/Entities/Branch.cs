using Flex.Shared.SeedWork.Workflow;

namespace Flex.System.Api.Entities
{
    public class Branch : ApprovalEntityBase<long>
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public int BranchType { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
    }
}
