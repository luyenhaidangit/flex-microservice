using System.Text.Json;
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
        
        // Additional fields from business requirements
        public DateTime? EstablishmentDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string ManagerName { get; set; } = default!;
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
        
        // Additional fields from business requirements
        public string ManagerName { get; set; } = default!;
        public DateTime? EstablishmentDate { get; set; }
    }

    public class CreateBranchRequestDto
    {
        [Required(ErrorMessage = "Branch code is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Branch code must be between 3 and 50 characters")]
        [RegularExpression("^[a-zA-Z0-9-_]+$", ErrorMessage = "Branch code can only contain letters, numbers, hyphens and underscores")]
        public string Code { get; set; } = default!;
        
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Branch name must be between 3 and 200 characters")]
        public string Name { get; set; } = default!;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = default!;
        
        [Required(ErrorMessage = "Member code is required")]
        [StringLength(50, ErrorMessage = "Member code cannot exceed 50 characters")]
        public string MemberCode { get; set; } = default!;
        
        [Required(ErrorMessage = "Branch type is required")]
        [StringLength(50, ErrorMessage = "Branch type cannot exceed 50 characters")]
        public string BranchType { get; set; } = default!;
        
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; } = default!;
        
        [StringLength(20, ErrorMessage = "Province code cannot exceed 20 characters")]
        public string ProvinceCode { get; set; } = default!;
        
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; } = default!;
        
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = default!;
        
        [StringLength(50, ErrorMessage = "Tax code cannot exceed 50 characters")]
        public string TaxCode { get; set; } = default!;
        
        [StringLength(100, ErrorMessage = "License number cannot exceed 100 characters")]
        public string LicenseNumber { get; set; } = default!;
        
        public DateTime? LicenseDate { get; set; }
        
        // Additional fields from business requirements
        public DateTime? EstablishmentDate { get; set; }
        
        [Required(ErrorMessage = "Manager name is required")]
        [StringLength(200, ErrorMessage = "Manager name cannot exceed 200 characters")]
        public string ManagerName { get; set; } = default!;
    }

    public class UpdateBranchRequestDto
    {
        [Required(ErrorMessage = "Branch name is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Branch name must be between 3 and 200 characters")]
        public string Name { get; set; } = default!;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = default!;
        
        [Required(ErrorMessage = "Member code is required")]
        [StringLength(50, ErrorMessage = "Member code cannot exceed 50 characters")]
        public string MemberCode { get; set; } = default!;
        
        [Required(ErrorMessage = "Branch type is required")]
        [StringLength(50, ErrorMessage = "Branch type cannot exceed 50 characters")]
        public string BranchType { get; set; } = default!;
        
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; } = default!;
        
        [StringLength(20, ErrorMessage = "Province code cannot exceed 20 characters")]
        public string ProvinceCode { get; set; } = default!;
        
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; } = default!;
        
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = default!;
        
        [StringLength(50, ErrorMessage = "Tax code cannot exceed 50 characters")]
        public string TaxCode { get; set; } = default!;
        
        [StringLength(100, ErrorMessage = "License number cannot exceed 100 characters")]
        public string LicenseNumber { get; set; } = default!;
        
        public DateTime? LicenseDate { get; set; }
        
        // Additional fields from business requirements
        public DateTime? EstablishmentDate { get; set; }
        
        [Required(ErrorMessage = "Manager name is required")]
        [StringLength(200, ErrorMessage = "Manager name cannot exceed 200 characters")]
        public string ManagerName { get; set; } = default!;
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
