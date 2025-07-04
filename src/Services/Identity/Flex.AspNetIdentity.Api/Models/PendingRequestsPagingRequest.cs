using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Models
{
    public class PendingRequestsPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        
        /// <summary>
        /// Filter theo loại yêu cầu: "Create", "Update", "Delete", "All"
        /// </summary>
        public string? RequestType { get; set; }
        
        /// <summary>
        /// Filter theo trạng thái: "Pending", "Draft", "All"
        /// </summary>
        public string? Status { get; set; }
        
        protected override Dictionary<string, string> OrderByMappings => new()
        {
            { "REQUESTED_DATE", "RequestedDate" },
            { "REQUEST_TYPE", "RequestType" },
            { "STATUS", "Status" }
        };
    }
} 