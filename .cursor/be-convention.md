# BE Convention

## Khởi tạo String Properties

**Model/DTO/ViewModel**: `string.Empty`
```csharp
public string UserName { get; set; } = string.Empty;
```

**Entity/View (DB connected)**: `default!`
```csharp
public string UserName { get; set; } = default!;
```

**Nullable strings**: `?` (không khởi tạo)
```csharp
public string? FullName { get; set; }
```