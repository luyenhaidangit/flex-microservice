namespace Flex.AspNetIdentity.Api.Models
{
    public class RolePagingDto
    {
        public long? Id { get; set; }
        public string? Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public bool? IsActive { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string Status { get; set; }
        public string? RequestType { get; set; }

        // Request ID for pending requests
        public long? RequestId { get; set; }

        // Thông tin trace/audit
        public string? RequestedBy { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }

    public class RoleApprovedListItemDto
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
