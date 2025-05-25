namespace Flex.AspNetIdentity.Api.Models
{
    public class RolePagingDto
    {
        public long? Id { get; set; }                      // ID của Role hoặc RoleRequest
        public string? Name { get; set; } = default!;      // Tên hiển thị
        public string Code { get; set; } = default!;      // Mã hệ thống (SystemName)
        public bool? IsActive { get; set; }               // Trạng thái dùng được không (cho Role "live")
        public string? Description { get; set; } = string.Empty;
        public string Status { get; set; } = "LIVE";      // LIVE / PENDING / REJ / CAN

        public string? RequestType { get; set; }

        public string? Maker { get; set; }                // Người tạo (từ RoleRequest)
        public string? Checker { get; set; }              // Người duyệt (từ RoleRequest)
        public DateTime CreatedDate { get; set; }         // Ngày tạo
        public DateTime? ApprovedDate { get; set; }       // Ngày duyệt (nếu có)

        public bool IsRequest => Status != "LIVE";        // Flag phụ để UI phân biệt loại bản ghi
    }
}
