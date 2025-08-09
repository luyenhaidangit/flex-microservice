## Quản lý cấu hình (Config Management)

- Backend chịu trách nhiệm validate token, FE chỉ giữ và gửi token.
- Cấu hình auth mode nên được cache tại backend, không bắt FE gọi mỗi lần reload.
- Nếu cần hiển thị khác nhau trên FE theo auth mode (VD: UI login thay đổi), thì:
- FE gọi /api/auth/config khi vào trang login (không cần gọi ở mọi trang).
- Backend trả cấu hình đã cache (DB → Memory/Redis).

### 1) Mục tiêu & phạm vi
- Đảm bảo FE có thể lấy cấu hình xác thực/ứng dụng thống nhất, nhanh và ổn định.
- Giảm số lần truy vấn DB cho cấu hình lặp lại bằng cache (Memory/Redis).
- Hạn chế rò rỉ thông tin nhạy cảm qua API cấu hình.

### 2) Nguồn sự thật (Source of Truth)
- Cấu hình gốc lưu ở DB (bảng cấu hình hệ thống), được cập nhật bởi System Admin.
- Backend là nơi duy nhất tổng hợp và phân phối cấu hình đến FE.

### 3) Kiến trúc cache
- Pattern: Cache-Aside (read-through). Lần đầu đọc từ DB → đẩy vào cache → trả về FE.
- Layer cache:
  - In-memory (per instance) cho key nhỏ, truy cập rất thường xuyên.
  - Redis (distributed) cho chia sẻ giữa nhiều instance/pod.
- TTL khuyến nghị: 60–300 giây, kèm random ±10% để tránh cache stampede.
- Key naming: `config:auth:mode`, `config:auth:policy:v2`, `config:ui:login`.

Tham khảo chi tiết trong `docs/guidelines/CACHE_GUIDELINE.md`.

### 4) API cho FE
- Endpoint: `GET /api/auth/config`
  - Thời điểm gọi: FE chỉ gọi khi vào trang login (không cần gọi ở mọi trang).
  - Client cache: có thể đặt `Cache-Control: max-age=60` nếu phù hợp.
  - Response (ví dụ):
    ```json
    {
      "mode": "password",          
      "loginHints": ["username", "email"],
      "passwordPolicy": {
        "minLength": 8,
        "requireUpper": true,
        "requireLower": true,
        "requireNumber": true,
        "requireSpecial": false
      },
      "features": {
        "twoFactor": false,
        "sso": false
      }
    }
    ```

### 5) Quy tắc dữ liệu trả về
- Không trả secrets/connection strings/keys.
- Chỉ trả cấu hình cần cho UI hiển thị/behavior (mode, hints, feature flags ở mức UI).
- Giá trị phải ổn định trong TTL; thay đổi tức thời phải đi kèm cơ chế invalidate cache.

### 6) Invalidation & cập nhật
- Khi cấu hình thay đổi bởi Admin:
  - Ghi DB → xóa/invalidate key Redis liên quan (`config:*`)
  - Xóa cache in-memory (nếu có cơ chế event/bus, phát sự kiện để các instance tự clear).
- Tự động hết hạn theo TTL để eventual consistency.

### 7) Bảo mật
- API `GET /api/auth/config` có thể public/anonymous nếu không chứa thông tin nhạy cảm.
- Nếu có field nhạy cảm (policy nội bộ), yêu cầu JWT và role phù hợp.
- Thêm rate-limit cơ bản để tránh lạm dụng.

### 8) Quan hệ với xác thực
- Backend chịu trách nhiệm validate token; FE chỉ giữ và gửi token kèm request.
- Cấu hình auth mode cần cache tại backend; FE không phải gọi mọi lần reload.

### 9) Trình tự FE đề xuất
- Vào trang login → gọi `GET /api/auth/config` → render UI theo `mode` và `features`.
- Sau khi login thành công, FE không cần gọi lại trừ khi user quay lại trang login.

### 10) Giám sát & audit
- Log cache hit/miss cho các key `config:*` (mức INFO/DEBUG tuỳ môi trường).
- Metric: cache hit rate, latency, lỗi kết nối Redis.
- Audit thay đổi cấu hình: ai thay đổi, lúc nào, giá trị trước/sau (ở backend).

### 11) Checklist nhanh
- Đã có TTL và key prefix rõ ràng cho config?
- Đã tách field nhạy cảm khỏi payload trả cho FE?
- Đã có cơ chế invalidate khi admin thay đổi cấu hình?
- Đã có log/metrics cơ bản cho cache?
