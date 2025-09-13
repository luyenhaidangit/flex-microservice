namespace Flex.AspNetIdentity.Api.Models.User
{
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
