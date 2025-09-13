# BE Convention

## Khởi tạo String Properties

**Model/DTO/ViewModel**: `string.Empty`
Thường nằm trong các folder Model/DTO/ViewModel
```csharp
public string UserName { get; set; } = string.Empty;
```

**Entity/View (DB connected)**: `default!`
Thường nằm trong các folder Entity/View
```csharp
public string UserName { get; set; } = default!;
```

**Nullable strings**: `?` (không khởi tạo)
```csharp
public string? FullName { get; set; }
```

## ValidationException và ErrorCode

### Quy trình thêm ErrorCode

**1. Thêm ErrorCode trong BE**
```csharp
// ErrorCode.cs
public const string USER_NOT_FOUND = "USER_NOT_FOUND";
public const string USER_ALREADY_EXISTS = "USER_ALREADY_EXISTS";
```

**2. Sử dụng ValidationException trong Service**
```csharp
if (user == null)
{
    throw new ValidationException(ErrorCode.USER_NOT_FOUND);
}
```

**3. Thêm bản dịch FE**
```json
// en.json
"errors": {
  "USER_NOT_FOUND": "User not found with the specified ID"
}

// vi.json  
"errors": {
  "USER_NOT_FOUND": "Không tìm thấy người dùng với ID đã chỉ định"
}
```

### Quy tắc
- **Format**: `{MODULE}_{ACTION}_{CONDITION}` (VD: `USER_NOT_FOUND`)
- **Naming**: UPPER_SNAKE_CASE, ngắn gọn rõ ràng
- **Grouping**: Theo module (`USER_*`, `ROLE_*`) và action (`*_NOT_FOUND`, `*_ALREADY_EXISTS`)

