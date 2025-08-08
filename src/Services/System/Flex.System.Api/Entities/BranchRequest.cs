using Flex.Shared.SeedWork.Workflow;

namespace Flex.System.Api.Entities
{
    public class BranchRequest : RequestBase<long>
    {
        public string EntityCode { get; set; } = default!;
        public string? OriginalData { get; set; }
    }
}
