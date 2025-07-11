# Cập nhật UpdateRoleDto và các DTO liên quan 🔄

## 📋 Tóm tắt thay đổi

### 1. **UpdateRoleDto** - Đã cập nhật hoàn toàn
```csharp
public class UpdateRoleDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = default!;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public List<ClaimDto>? Claims { get; set; }
}
```

### 2. **CreateRoleDto** - Đã cập nhật để nhất quán
```csharp
public class CreateRoleDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string Code { get; set; } = default!;

    [StringLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public List<ClaimDto>? Claims { get; set; }

    public string? Status { get; set; } // Draft hoặc Pending
}
```

### 3. **RoleDto** - Đã thêm trường Claims
```csharp
public class RoleDto
{
    // ... các trường khác
    public List<ClaimDto>? Claims { get; set; }
    // ... các trường khác
}
```

### 4. **ClaimDto** - Đã tồn tại và được sử dụng
```csharp
public class ClaimDto
{
    public string Type { get; set; } = "permission";
    public string Value { get; set; } = default!;
}
```

## 🔧 Các thay đổi trong RoleService

### 1. **GetRoleRequestDetailAsync**
- Cập nhật để xử lý `ClaimDto` thay vì `List<string>`
- Trả về permissions dưới dạng `"Type:Value"`

### 2. **CreateDeleteRoleRequestAsync**
- Lưu claims vào snapshot khi tạo delete request
- Chuyển đổi từ `Claim` sang `ClaimDto`

### 3. **GetRoleByIdAsync & GetRoleByCodeAsync**
- Trả về claims khi lấy thông tin role
- Xử lý claims cho cả pending request và approved role

## 📝 Validation Rules

### UpdateRoleDto & CreateRoleDto
- `Name`: Required, max 100 ký tự
- `Code`: Required, max 50 ký tự  
- `Description`: Optional, max 500 ký tự
- `IsActive`: Boolean, default true
- `Claims`: Optional, List<ClaimDto>

## 🎯 Lợi ích

1. **Tính nhất quán**: Tất cả DTO đều sử dụng `ClaimDto` thay vì `List<string>`
2. **Validation**: Thêm data annotations cho validation
3. **Type Safety**: Sử dụng strongly-typed objects
4. **Flexibility**: Claims có thể có Type và Value riêng biệt
5. **Maintainability**: Code dễ bảo trì và mở rộng

## ⚠️ Lưu ý

- Đảm bảo frontend gửi claims dưới dạng `ClaimDto[]` thay vì `string[]`
- Claims được format dưới dạng `"Type:Value"` trong API response
- Tất cả API liên quan đến role đều đã được cập nhật để hỗ trợ claims 