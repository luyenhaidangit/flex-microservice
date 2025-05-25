namespace Flex.AspNetIdentity.Api.Models
{
    public class RoleDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = default!;

        public string Code { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string SystemName { get; set; } = default!; // Tên định danh hệ thống

        public bool IsSystemRole { get; set; } // true nếu là role hệ thống (không thể sửa/xóa)

        public bool IsActive { get; set; } // có đang kích hoạt không

        public DateTime CreatedDate { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string? LastModifiedBy { get; set; }

        public List<ClaimDto> Claims { get; set; } = new(); // Danh sách quyền nếu cần
    }
}
