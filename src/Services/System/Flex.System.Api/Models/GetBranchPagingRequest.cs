using Flex.Shared.SeedWork;

namespace Flex.System.Api.Models
{
    public class GetBranchPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public string? IsActive { get; set; }
        public string? RequestType { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
            { "code", "Code" },
            { "name", "Name" },
            { "description", "Description" },
            { "status", "Status" },
            { "createdDate", "CreatedDate" }
        };
    }
}
