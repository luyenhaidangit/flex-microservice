using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Models
{
    public class GetRolesPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public string? IsActive { get; set; }
        public string? Type { get; set; }
  
        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
