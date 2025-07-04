namespace Flex.AspNetIdentity.Api.Models
{
    public class RoleComparisonDto
    {
        public long RequestId { get; set; }
        public string RequestType { get; set; } = default!; // CREATE / UPDATE / DELETE
        public string RequestedBy { get; set; } = default!;
        public DateTime? RequestedDate { get; set; }
        
        // Bản chính hiện tại (nếu có)
        public RoleDto? CurrentVersion { get; set; }
        
        // Bản nháp đề xuất
        public RoleDto? ProposedVersion { get; set; }
        
        // Danh sách các trường thay đổi
        public List<FieldDiffDto> Changes { get; set; } = new();
    }
    
    public class FieldDiffDto
    {
        public string FieldName { get; set; } = default!;
        public string? CurrentValue { get; set; }
        public string? ProposedValue { get; set; }
        public string ChangeType { get; set; } = default!; // "Added", "Modified", "Removed"
    }
} 