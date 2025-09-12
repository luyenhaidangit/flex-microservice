namespace Flex.AspNetIdentity.Api.Models.User
{
    /// <summary>
    /// DTO cho API xem chi tiết user request trong modal
    /// </summary>
    public class UserRequestDetailDto
    {
        public string RequestId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // CREATE / UPDATE / DELETE
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
        public UserDetailDataDto? OldData { get; set; } // Dữ liệu cũ (cho UPDATE/DELETE)
        public UserDetailDataDto? NewData { get; set; } // Dữ liệu mới (cho CREATE/UPDATE)
    }

    /// <summary>
    /// DTO cho dữ liệu user chi tiết trong modal
    /// </summary>
    public class UserDetailDataDto
    {
        public string UserName { get; set; } = string.Empty;
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
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
        public DateTime ApprovedDate { get; set; }
        public string? Comment { get; set; }
        public long? CreatedUserId { get; set; } // For CREATE requests
    }
}
