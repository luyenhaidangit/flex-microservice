using Flex.Shared.SeedWork;

namespace Flex.Shared.DTOs.System.Department
{
    public class GetDepartmentsPagingRequest : PagingRequest
    {
        public string? Name { get; set; }
        public string? Status { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
            { "NAME", "NAME" },
            { "STATUS", "STATUS" },
        };
    }
}
