using Flex.Contracts.Domains;
using Flex.Shared.Constants.Common;
using Flex.Shared.Constants.System.Branch;

namespace Flex.System.Api.Entities
{
    public class Branch : EntityBase<long>
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public int BranchType { get; set; } = BranchTypeConstants.Branch;
        public bool IsActive { get; set; } = true;
        public required string Status { get; set; } = StatusConstant.Approved;
        public string? Description { get; set; }
    }
}
