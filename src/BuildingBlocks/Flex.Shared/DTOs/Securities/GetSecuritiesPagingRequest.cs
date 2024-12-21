using Flex.Shared.SeedWork;

namespace Flex.Shared.DTOs.Securities
{
    public class GetSecuritiesPagingRequest : PagingRequest
    {
        public string? Symbol { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
