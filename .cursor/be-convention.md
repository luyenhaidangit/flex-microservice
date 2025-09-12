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