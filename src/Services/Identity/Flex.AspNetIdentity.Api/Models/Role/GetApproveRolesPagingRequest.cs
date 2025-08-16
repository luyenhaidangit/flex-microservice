using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Models.Role
{
    /// <summary>
    /// DTO for paging roles
    /// </summary>
    public class GetApproveRolesPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public string? IsActive { get; set; }
        public string? Type { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
