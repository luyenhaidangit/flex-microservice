namespace Flex.AspNetIdentity.Api.Models
{


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

    /// <summary>
    /// DTO for role approval result
    /// </summary>
    public class RoleApprovalResultDto
    {
        public long RequestId { get; set; }
        public string RequestType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string ApprovedBy { get; set; } = default!;
        public DateTime ApprovedDate { get; set; }
        public string? Comment { get; set; }
        public long? CreatedRoleId { get; set; } // For CREATE requests
    }
}
