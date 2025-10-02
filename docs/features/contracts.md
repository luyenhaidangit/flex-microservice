# Contracts Architecture - Flex Microservice

## Tổng quan

Trong kiến trúc microservice Flex, Contracts là các thư viện chỉ chứa thông điệp chung (event/command DTO) để các service khác cùng hiểu được message, không chứa logic hay domain riêng của bất kỳ service nào.

## Nguyên tắc thiết kế

### Dependency Direction
- **Service → Contracts**: Các service sẽ tham chiếu đến project Contracts để publish/consume message
- **Contracts x→ Service**: Project Contracts không được tham chiếu ngược lại vào bất kỳ service nào

> Điều này tránh việc Contracts "biết" logic hay phụ thuộc vào domain của một service cụ thể, từ đó làm Contracts trở thành bị khóa chặt (tight coupling).

### Nội dung Contracts
- Chỉ chứa dữ liệu cần giao tiếp, là DTO phẳng
- Không có EF model, không có logic domain, không có service reference
- Service nào cần publish thì map từ domain sang contract bằng AutoMapper hoặc thủ công

## Cấu trúc Contracts trong Flex

### 1. Integration Events (Flex.EventBus.Messages)

Biểu diễn sự kiện đã xảy ra trong hệ thống, tên ở dạng quá khứ (Past tense).

**Base Class:**
```csharp
namespace Flex.EventBus.Messages
{
    public record IntegrationBaseEvent : IIntegrationEvent
    {
        public DateTime CreationDate { get; } = DateTime.UtcNow;
        public Guid Id { get; } = Guid.NewGuid();
    }
}
```

**Ví dụ thực tế:**
```csharp
namespace Flex.EventBus.Messages.IntegrationEvents.Events
{
    /// <summary>
    /// Event được publish khi Branch được tạo mới trong System service
    /// </summary>
    public record BranchCreatedEvent : IntegrationBaseEvent, IBranchCreatedEvent
    {
        public long BranchId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int BranchType { get; set; }
        public bool IsActive { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = default!;
    }
}
```

### 2. Domain Events (Flex.Contracts.Events)

Biểu diễn sự kiện trong domain cụ thể, sử dụng MediatR pattern.

**Base Class:**
```csharp
namespace Flex.Contracts.Common.Events
{
    public abstract class BaseEvent : INotification
    {
    }
}
```

**Ví dụ thực tế:**
```csharp
namespace Flex.Contracts.Events.Users
{
    public sealed record UserCreatedApprovedEvent(
        Guid UserId,
        string Email,
        string FullName,
        string Language,
        string ActivationLink,
        DateTime ApprovedAtUtc
    );
}
```

### 3. Command DTO (Command Messages)

Biểu diễn yêu cầu hành động từ service này đến service khác, tên ở dạng mệnh lệnh (Imperative).

**Ví dụ:**
```csharp
namespace Flex.Identity.Contracts.Users.V1;

public sealed record CreateUserRequested(
    Guid MessageId,
    string Email,
    string FullName,
    long BranchId,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    string? CorrelationId = null
);
```

### 4. Response DTO (Request-Response)

Chỉ khi cần reply qua RabbitMQ (MassTransit có RequestClient<T>).

**Ví dụ:**
```csharp
namespace Flex.Identity.Contracts.Users.V1;

public sealed record CreateUserResponse(
    Guid UserId,
    bool Success,
    string? Reason,
    string? ErrorCode = null
);
```

### 5. Enum/Constant dùng chung

Trạng thái, loại lệnh, các giá trị chung không có logic tính toán.

**Ví dụ:**
```csharp
namespace Flex.Shared.Enums;

public enum UserStatus
{
    Unauthorised = 0,
    Authorised = 1,
    Locked = 2,
    Disabled = 3
}

public enum RequestStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2
}
```

### 6. Metadata/Context

Chuẩn hóa traceId, correlationId, tenantId cho toàn hệ thống.

**Ví dụ:**
```csharp
namespace Flex.Contracts.Common;

public record MessageContext(
    string CorrelationId,
    string TraceId,
    string? TenantId = null,
    string? UserId = null
);
```

## Naming Conventions

### Events
- **Integration Events**: `{Entity}{Action}Event` (ví dụ: `BranchCreatedEvent`)
- **Domain Events**: `{Entity}{Action}Event` (ví dụ: `UserCreatedApprovedEvent`)

### Commands
- **Commands**: `{Action}{Entity}Requested` (ví dụ: `CreateUserRequested`)
- **Responses**: `{Action}{Entity}Response` (ví dụ: `CreateUserResponse`)

### Namespaces
- **Integration Events**: `Flex.EventBus.Messages.IntegrationEvents.Events`
- **Domain Events**: `Flex.Contracts.Events.{Domain}`
- **Commands**: `Flex.{Service}.Contracts.{Domain}.V{Version}`
- **Enums**: `Flex.Shared.Enums`

## Best Practices

### 1. Immutability
- Sử dụng `record` cho tất cả contracts
- Properties chỉ có getter hoặc init-only setter

### 2. Versioning
- Sử dụng versioning trong namespace (V1, V2, ...)
- Không thay đổi contract cũ, chỉ thêm version mới

### 3. Documentation
- Thêm XML documentation cho tất cả contracts
- Mô tả rõ mục đích và cách sử dụng

### 4. Validation
- Contracts không chứa validation logic
- Validation được thực hiện ở service layer

### 5. Serialization
- Sử dụng System.Text.Json cho serialization
- Đảm bảo contracts có thể serialize/deserialize đúng cách

## Cấu trúc thư mục

```
src/BuildingBlocks/
├── Flex.Contracts/                    # Domain events và base contracts
│   ├── Common/
│   │   ├── Events/
│   │   └── Interfaces/
│   └── Domains/
├── Flex.Contracts.Events/            # Specific domain events
│   └── Users/
├── Flex.EventBus.Messages/           # Integration events
│   ├── IntegrationEvents/
│   │   ├── Events/
│   │   └── Interfaces/
│   └── IIntegrationEvent.cs
└── Flex.Contracts.Grpc/             # gRPC contracts
    └── Protos/
```

## Migration Strategy

Khi cần thay đổi contract:

1. **Backward Compatible**: Thêm properties mới với default values
2. **Breaking Changes**: Tạo version mới (V2, V3, ...)
3. **Deprecation**: Đánh dấu contract cũ là obsolete
4. **Cleanup**: Xóa contract cũ sau khi tất cả consumers đã migrate