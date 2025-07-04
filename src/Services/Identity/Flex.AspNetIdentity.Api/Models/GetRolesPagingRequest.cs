using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Models
{
    public class GetRolesPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        
        /// <summary>
        /// Filter theo trạng thái: "Approved", "Pending", "Draft", "All"
        /// </summary>
        public string? Status { get; set; }
        
        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
