using Flex.Shared.SeedWork;

namespace Flex.System.Api.Models.Branch
{
    public class GetBranchPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public string? IsActive { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
