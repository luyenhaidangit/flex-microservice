using System.Text.Json;

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
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = default!;
        
        // Additional fields for Branch
        public string MemberCode { get; set; } = default!;
        public string BranchType { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string ProvinceCode { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string TaxCode { get; set; } = default!;
        public string LicenseNumber { get; set; } = default!;
        public DateTime? LicenseDate { get; set; }
    }

    public class BranchListItemDto
    {
        public long Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Status { get; set; } = default!;
        public bool IsActive { get; set; }
        
        // Additional fields for Branch
        public string MemberCode { get; set; } = default!;
        public string BranchType { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string ProvinceCode { get; set; } = default!;
    }

    public class CreateBranchRequestDto
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string MemberCode { get; set; } = default!;
        public string BranchType { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string ProvinceCode { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string TaxCode { get; set; } = default!;
        public string LicenseNumber { get; set; } = default!;
        public DateTime? LicenseDate { get; set; }
    }

    public class UpdateBranchRequestDto
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string MemberCode { get; set; } = default!;
        public string BranchType { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string ProvinceCode { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string TaxCode { get; set; } = default!;
        public string LicenseNumber { get; set; } = default!;
        public DateTime? LicenseDate { get; set; }
    }

    public class DeleteBranchRequestDto
    {
        public string Reason { get; set; } = default!;
    }

    public class BranchPendingPagingDto
    {
        public long Id { get; set; }
        public string EntityCode { get; set; } = default!;
        public string Action { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
    }

    public class BranchRequestDetailDto
    {
        public long Id { get; set; }
        public string Action { get; set; } = default!;
        public long EntityId { get; set; }
        public string EntityCode { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public string? CheckerId { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string? Comments { get; set; }
        public BranchDto? RequestData { get; set; }
        public BranchDto? OriginalData { get; set; }
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
