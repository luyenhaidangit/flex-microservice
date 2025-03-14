using Flex.Shared.Enums.General;
using Flex.Shared.SeedWork;

namespace Flex.Shared.DTOs.System
{
    public class GetConfigsPagingRequest : PagingRequest
    {
        public string? Key { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
            { "KEY", "KEY" },
            { "DATE", "LASTMODIFIEDDATE" }
        };
    }
}
