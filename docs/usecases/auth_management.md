# Phân quyền và xác thực

1. Tổng quan luồng
- User login (username/password) → API .NET xác thực và phát JWT (thời hạn ngắn, ví dụ 10–15 phút).
- Angular lưu token (in-memory, hoặc tạm sessionStorage) và đính kèm Authorization: Bearer … vào mọi request.
- API validate JWT và Authorize theo claims/role.
- Hết hạn token ⇒ 401 → FE chuyển về trang login. (Không dùng refresh, đăng nhập lại.)
2. Ứng xử khi người dùng reload trang (FE)
2.1. Luồng khởi động (bootstrap)
- Đảm bảo gọi api /me mỗi lần reload để thực hiện validate token.
- FE đọc accessToken từ localStorage.
- Giải mã phần payload (exp) để kiểm tra hạn.
- Nếu token hết hạn hoặc không có → chuyển /login và xóa token còn sót.
- Nếu token còn hạn → nạp vào in-memory và tiếp tục khởi động app.
- Đặt auto-logout timer: tự đăng xuất ~30–60 giây trước khi token hết hạn.
2.2. Luồng khi đang dùng
- Mọi request đính kèm Authorization: Bearer <token>.
- Nếu API trả 401 → xóa token, chuyển /login.
- Đồng bộ đa tab: lắng nghe sự kiện storage để khi một tab logout thì các tab khác cũng logout.
3. Chính sách localStorage (chấp nhận trade-off) & bảo mật
- Cảnh báo: localStorage dễ bị lộ nếu có XSS, vì JS đọc được. Dưới đây là tối thiểu cần làm để giảm rủi ro.
- TTL ngắn cho access token (10–15 phút).
- CSP nghiêm ngặt:
+ default-src 'self'
+ script-src 'self' 'nonce-<random>' (không dùng unsafe-inline, không eval)
- Không chèn HTML thô (innerHTML) trừ khi đã sanitize.
- Sanitization: kiểm tra tất cả chỗ render HTML, link, query string… sử dụng các API an toàn của Angular.
- Xóa sạch token khi: logout, 401, tài khoản bị khóa, đổi mật khẩu.
- Đa tab: dùng storage event để phát tán logout.
- Không mã hóa token trong localStorage: “tự mã hóa” bằng JS không giúp chống XSS (attacker vẫn dùng JS đọc/decrypt).
- Không lưu refresh token ở localStorage (mình đang không dùng refresh).
- Khi cần nâng cấp an toàn/UX: chuyển sang in-memory access token + refresh bằng HttpOnly cookie hoặc BFF.