namespace Flex.AspNetIdentity.Api.Models
{
    public class RoleRequestDto
    {
        public long RequestId { get; set; }

        public long? RoleId { get; set; } // null nếu là CREATE

        public string RequestType { get; set; } = default!; // CREATE / UPDATE / DELETE

        public string Status { get; set; } = default!; // PENDING / APPROVED / REJECTED

        public string RequestedBy { get; set; } = default!;

        public DateTime RequestedDate { get; set; }

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? RejectReason { get; set; }

        public string? Comment { get; set; }

        public RoleDto? ProposedData { get; set; } // Dữ liệu role được đề xuất (deserialize từ JSON)
    }
}
