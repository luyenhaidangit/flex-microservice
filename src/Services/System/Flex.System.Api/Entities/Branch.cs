using Flex.Contracts.Domains;
using Flex.Shared.Constants.Common;

namespace Flex.System.Api.Entities
{
    public class Branch : EntityBase<long>
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        
        // Workflow status field
        public string Status { get; set; } = StatusConstant.Approved;
        
        // Additional fields for Branch management
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
        public DateTime? EstablishmentDate { get; set; } // Ngày thành lập
        public DateTime? ClosedDate { get; set; } // Ngày ngừng hoạt động
        public string ManagerName { get; set; } = default!; // Người quản lý chính
    }
}
