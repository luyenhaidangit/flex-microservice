using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Models.User
{
    public class GetUsersPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public long? BranchId { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}