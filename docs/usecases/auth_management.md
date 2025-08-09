# Phân quyền và xác thực

1. Tổng quan luồng
- User login (username/password) → API .NET xác thực và phát JWT (thời hạn ngắn, ví dụ 10–15 phút).
- Angular lưu token (in-memory, hoặc tạm sessionStorage) và đính kèm Authorization: Bearer … vào mọi request.
- API validate JWT và Authorize theo claims/role.
- Hết hạn token ⇒ 401 → FE chuyển về trang login. (Không dùng refresh, đăng nhập lại.)