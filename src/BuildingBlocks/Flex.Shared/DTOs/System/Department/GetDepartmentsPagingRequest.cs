using Flex.Shared.SeedWork;

namespace Flex.Shared.DTOs.System.Department
{
    public class GetDepartmentsPagingRequest : PagingRequest
    {
        public string? Key { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
            { "KEY", "KEY" },
            { "DATE", "LASTMODIFIEDDATE" }
        };
    }
}
