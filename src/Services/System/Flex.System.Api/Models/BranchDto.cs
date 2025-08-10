using System.ComponentModel.DataAnnotations;

namespace Flex.System.Api.Models
{
    public class BranchDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Status { get; set; } = default!;
        public bool IsActive { get; set; }
    }

    public class BranchListItemDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Status { get; set; } = default!;
        public bool IsActive { get; set; }
    }

    public class UpdateBranchRequestDto
    {
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Branch name must be between 3 and 200 characters")]
        public string Name { get; set; } = default!;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = default!;
    }

    public class DeleteBranchRequestDto
    {
        public string Reason { get; set; } = default!;
    }

    public class BranchRequestDetailDto
    {
        public long Id { get; set; }
        public string Action { get; set; } = default!;
        public long EntityId { get; set; }
        //public string EntityCode { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
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
        public string RequestType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string ApprovedBy { get; set; } = default!;
        public DateTime ApprovedDate { get; set; }
        public string Comment { get; set; } = default!;
    }
}
