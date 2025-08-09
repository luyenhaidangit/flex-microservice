### AI File Generation Rules (Best Practices) — Flex Microservice (.NET 9)

Tài liệu này hướng dẫn quy tắc khi sinh mã/tạo file bằng AI cho dự án Flex Microservice, đảm bảo tuân thủ kiến trúc microservices, chuẩn .NET 9 và các conventions hiện có trong `.cursorrules`.

### Nguyên tắc vàng
- **Tôn trọng kiến trúc**: Mỗi service là một container riêng; không đan xen code giữa services. Building Blocks chỉ chứa shared components.
- **Theo conventions**: Tên project `Flex.{Service}.{Layer}`, container viết thường, sử dụng biến môi trường cho secrets (không hardcode).
- **Không lặp lại constants**: Tìm constants trước khi hardcode. Ưu tiên dùng constants trong `src/BuildingBlocks/Flex.Shared/Constants/`.
- **Không log secrets**: Không log password/token/connection string; dùng SeriLog structured logging.
- **Idempotent và DI**: Tất cả services/repositories phải đăng ký DI đúng lifetime (Scoped cho DB).
- **Async/await**: Luôn dùng bất đồng bộ; tránh `.Result`/`.Wait()`.
- **Validation rõ ràng**: Dùng FluentValidation khi có; ModelState cho controllers đã bật theo `[ApiController]`.

### Vị trí file khi tạo mới
- **Entities**: `src/Services/{ServiceName}/{Project}.Api/Entities/`
- **EF Configurations**: `.../Data/Configurations/`
- **Repositories (Interfaces/Impl)**: `.../Repositories/Interfaces/`, `.../Repositories/`
- **Services (Interfaces/Impl)**: `.../Services/Interfaces/`, `.../Services/`
- **Controllers**: `.../Controllers/`
- **DTOs/Requests**: Nếu chia sẻ dùng nhiều service → `src/BuildingBlocks/Flex.Shared/DTOs/{Domain}`; nếu chỉ dùng nội bộ service → `.../Models/`
- **Validators**: `.../Validators/` (ví dụ: `Flex.System.Api/Validators/...Validator.cs`)

### Quy tắc đặt tên & cấu trúc
- **Class/Method**: PascalCase; biến camelCase. DTO kết thúc với `Dto` hoặc `Request`/`Response` rõ nghĩa.
- **Repository/Service**: Interface `I[Entity]Repository`, Impl `[Entity]Repository`; tương tự cho Service.
- **Controller**: `[EntityName]Controller` với route `api/[controller]`. Action trả `ActionResult<T>`.

### Workflow tạo mới Entity (rút gọn)
1) Tạo `Entity : EntityBase<long>` với các trường chuẩn (Code/Name/Description/Status/IsActive...).
2) Tạo `Configuration : IEntityTypeConfiguration<Entity>` (table plural, key, độ dài chuỗi, default values).
3) Tạo Repository (Interface + Impl) theo pattern trong `.cursorrules` (có paging Regular/Approved, `GetByCode`/`Exists...`).
4) Tạo Service (Interface + Impl) xử lý nghiệp vụ, validation, transaction (nếu có approve/reject).
5) Đăng ký DI trong `Extensions/ServiceExtensions.cs` → `AddScoped`.
6) Tạo Controller REST theo conventions (thêm XML comments cho Swagger nếu cần).

### Paging & Constants
- Tuân thủ mẫu trong `.cursorrules`/paging_patterns: validate pageIndex/pageSize, `EF.Functions.Like`, `WhereIf(...)`, `PagedResult<T>.Create(...)`.
- Luôn tra `StatusConstant`, `RequestTypeConstant`, ... thay vì hardcode.

### Validation (FluentValidation)
- Đặt trong `.../Validators`. Ví dụ: `LoginByUserNameRequestValidator : AbstractValidator<LoginByUserNameRequest>`.
- Quy tắc: `Cascade(CascadeMode.Stop)`, thông báo lỗi rõ ràng; không lộ dữ liệu nhạy cảm.
- Đăng ký quét assembly validators nếu áp dụng (hoặc để AspNet `[ApiController]` xử lý ModelState mặc định).

### Bảo mật & Config
- **JWT**: Sử dụng module `Flex.Security` và `JwtSettings` qua `IOptions<T>`. Không hardcode khóa.
- **Config**: Dùng `GetRequiredSection<T>`/`GetRequiredValue<T>` trong `ServiceConfigurationExtensions`. Secrets qua biến môi trường.
- **Input**: Sanitize/validate đầu vào; không trả lỗi chi tiết nội bộ cho client.

### Dữ liệu & EF Core (Oracle)
- Không truy vấn N+1; dùng `Select` projection hoặc `Include` khi cần.
- Sử dụng transaction cho luồng approve/reject; rollback khi lỗi.
- Không trả về entity trực tiếp cho API; map sang DTO.

### Logging & Monitoring
- SeriLog structured logging (`ILogger<T>`). Log mức `Information/Warning/Error`; không log secrets.
- Health checks qua service System khi cần; bật Swagger/OpenAPI cho API docs.

### Caching (Redis)
- Dùng `Flex.Redis`; key theo `CacheRedisKeyConstant`; set TTL hợp lý; invalidate khi dữ liệu thay đổi.

### Kiểm thử & chất lượng
- Thêm unit test khi tạo business logic quan trọng; test paging/filter/validation.
- Build xanh trước khi PR: `dotnet build Flex.sln`.
- Đảm bảo linter/style không lỗi; đặt code rõ ràng, dễ đọc.

### Commit & PR
- Commit message: ngắn gọn, mô tả thay đổi. Ví dụ: `feat(identity): add LoginByUserNameRequest validator`.
- PR mô tả impact, migration (nếu có), cách test, liên kết tài liệu liên quan.

### Trình tự sinh mã bằng AI trong Cursor (khuyến nghị)
- Trước khi tạo file: tìm DTO/constants/luồng sẵn có với tìm kiếm ngữ nghĩa; tuân thủ cấu trúc thư mục.
- Sinh mã theo từng bước logic (Entities → Configurations → Repositories → Services → DI → Controllers).
- Sau mỗi nhóm thay đổi: chạy build; với logic phức tạp thì thêm test nhanh.
- Không tạo mã không cần thiết; không thay đổi code không liên quan.

Tham chiếu: `.cursorrules` chứa chi tiết conventions, paging patterns và entity workflow đầy đủ.


