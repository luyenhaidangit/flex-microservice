 

namespace Flex.Notification.Api.Models
{
    public class BranchDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class UpdateBranchRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int BranchType { get; set; }
        public bool IsActive { get; set; }
    }

    public class DeleteBranchRequestDto
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class BranchRequestDetailDto
    {
        public long Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public long EntityId { get; set; }
        //public string EntityCode { get; set; } = default!;
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? CheckerId { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string? Comments { get; set; }
        public BranchDto? RequestData { get; set; }
        //public BranchDto? OriginalData { get; set; }
    }

    public class ApproveBranchRequestDto
    {
        public string? Comment { get; set; }
    }

    public class RejectBranchRequestDto
    {
        public string? Reason { get; set; }
    }

    public class BranchApprovalResultDto
    {
        public long RequestId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
        public DateTime ApprovedDate { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
