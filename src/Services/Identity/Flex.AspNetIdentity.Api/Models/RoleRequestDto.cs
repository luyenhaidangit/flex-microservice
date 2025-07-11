namespace Flex.AspNetIdentity.Api.Models
{
    public class RoleRequestDto
    {
        public long RequestId { get; set; }

        public long? RoleId { get; set; } // null nếu là CREATE

        public string RequestType { get; set; } = default!; // CREATE / UPDATE / DELETE

        public string Status { get; set; } = default!; // PENDING / APPROVED / REJECTED

        public string RequestedBy { get; set; } = default!;

        public DateTime? RequestedDate { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? RejectReason { get; set; }

        public string? Comment { get; set; }

        public RoleDto? ProposedData { get; set; } // Dữ liệu role được đề xuất (deserialize từ JSON)
    }

    /// <summary>
    /// DTO cho API xem chi tiết request trong modal
    /// </summary>
    public class RoleRequestDetailDto
    {
        public string RequestId { get; set; } = default!;
        public string Type { get; set; } = default!; // CREATE / UPDATE / DELETE
        public string CreatedBy { get; set; } = default!;
        public string CreatedDate { get; set; } = default!;
        public RoleDetailDataDto? OldData { get; set; } // Dữ liệu cũ (cho UPDATE/DELETE)
        public RoleDetailDataDto? NewData { get; set; } // Dữ liệu mới (cho CREATE/UPDATE)
    }

    /// <summary>
    /// DTO cho dữ liệu role chi tiết trong modal
    /// </summary>
    public class RoleDetailDataDto
    {
        public string RoleCode { get; set; } = default!;
        public string RoleName { get; set; } = default!;
        public string Description { get; set; } = default!;
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
