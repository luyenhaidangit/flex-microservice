using Flex.Shared.SeedWork;

namespace Flex.Notification.Api.Models.Branch
{
    public class GetBranchPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public string? IsActive { get; set; }
        public string? Type { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
