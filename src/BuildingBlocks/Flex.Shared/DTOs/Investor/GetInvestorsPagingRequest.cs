using Flex.Shared.SeedWork;

namespace Flex.Shared.DTOs.Investor
{
    public class GetInvestorsPagingRequest : PagingRequest
    {
        public string? Name { get; set; }

        public string? No { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}