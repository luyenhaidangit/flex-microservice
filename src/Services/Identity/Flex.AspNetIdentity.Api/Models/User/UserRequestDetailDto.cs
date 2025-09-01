namespace Flex.AspNetIdentity.Api.Models.User
{
    /// <summary>
    /// DTO cho API xem chi tiết user request trong modal
    /// </summary>
    public class UserRequestDetailDto
    {
        public string RequestId { get; set; } = default!;
        public string Type { get; set; } = default!; // CREATE / UPDATE / DELETE
        public string CreatedBy { get; set; } = default!;
        public string CreatedDate { get; set; } = default!;
        public UserDetailDataDto? OldData { get; set; } // Dữ liệu cũ (cho UPDATE/DELETE)
        public UserDetailDataDto? NewData { get; set; } // Dữ liệu mới (cho CREATE/UPDATE)
    }

    /// <summary>
    /// DTO cho dữ liệu user chi tiết trong modal
    /// </summary>
    public class UserDetailDataDto
    {
        public string UserName { get; set; } = default!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long? BranchId { get; set; }
        public string? BranchName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }

    /// <summary>
    /// DTO for user request approval result
    /// </summary>
    public class UserRequestApprovalResultDto
    {
        public long RequestId { get; set; }
        public string RequestType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string ApprovedBy { get; set; } = default!;
        public DateTime ApprovedDate { get; set; }
        public string? Comment { get; set; }
        public long? CreatedUserId { get; set; } // For CREATE requests
    }
}
