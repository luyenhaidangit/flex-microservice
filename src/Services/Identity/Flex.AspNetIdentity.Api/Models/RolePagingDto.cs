using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Models
{
    /// <summary>
    /// DTO for paging roles
    /// </summary>
    public class GetRolesPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public string? IsActive { get; set; }
        public string? Type { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }

    /// <summary>
    /// DTO for paging approved roles
    /// </summary>
    public class RoleApprovedListItemDto
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO for paging pending roles
    /// </summary>
    public class RolePendingPagingDto
    {
        // ===== Request information =====
        public long? RequestId { get; set; }

        // ===== Role information =====
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; } = string.Empty;
        public string RequestType { get; set; } = default!;
        public string? RequestedBy { get; set; }
        public DateTime? RequestedDate { get; set; }
    }
}
