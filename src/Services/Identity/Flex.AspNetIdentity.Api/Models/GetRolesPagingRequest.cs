using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Models
{
    public class GetRolesPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public string? IsActive { get; set; }
        
        /// <summary>
        /// Filter theo trạng thái: "Approved", "Pending", "Draft", "All"
        /// </summary>        
        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
