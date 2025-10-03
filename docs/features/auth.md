Xác thực & Phân quyền
======================

Tài liệu này mô tả tính năng xác thực (authentication) và phân quyền (authorization) cho hệ thống Flex Identity theo phong cách trình bày thực dụng: mục tiêu, thao tác chính, tham số lọc/sắp xếp, và API tham chiếu. Nội dung được viết tương tự cách trình bày bạn yêu cầu.

Mục tiêu
- Đăng nhập/đăng xuất an toàn bằng JWT Bearer.
- Làm mới token (access/refresh) và quản lý vòng đời phiên.
- Phân quyền theo vai trò và quyền hạn chi tiết (role/permission).
- Tìm kiếm, lọc, sắp xếp người dùng theo tiêu chí thường dùng để hỗ trợ cấp quyền nhanh.

Khái niệm chính
- Access Token: JWT ngắn hạn dùng gọi API bảo vệ.
- Refresh Token: mã dài hạn để xin Access Token mới khi gần hết hạn.
- Role: vai trò hệ thống (Admin, Operator, Viewer…).
- Permission/Claim: quyền chi tiết (theo chức năng/route/resource).
- Lockout: trạng thái khóa đăng nhập tạm thời hoặc vĩnh viễn.

Tác vụ thường dùng
- Đăng nhập: nhập `userName`/`password`, nhận `accessToken` + `refreshToken`.
- Làm mới token: gửi `refreshToken` hợp lệ để nhận `accessToken` mới.
- Đăng xuất: thu hồi refresh token hiện tại (revoke) và kết thúc phiên.
- Tra cứu người dùng để cấp quyền: tìm theo `userName`, họ tên, email, vai trò.
- Gán/bỏ vai trò cho người dùng: cập nhật role, kiểm tra hiệu lực quyền.
- Khóa/Mở khóa đăng nhập người dùng.
- Bật trạng thái “bắt buộc đổi mật khẩu lần đầu” nếu cần.

Luồng nghiệp vụ (tóm tắt)
1) Đăng nhập:
   - Người dùng cung cấp thông tin, hệ thống xác thực, phát hành access/refresh token.
   - Ghi nhận phiên và thông tin thiết bị (nếu bật), thiết lập thời hạn token.
2) Làm mới token:
   - Client gửi refresh token còn hiệu lực, hệ thống kiểm tra và phát hành access token mới.
3) Phân quyền:
   - API bảo vệ bởi JWT, kiểm tra role/permission trên mỗi request.
4) Quản lý người dùng phục vụ phân quyền:
   - Tìm kiếm, lọc, sắp xếp; gán/bỏ vai trò; khóa/mở khóa; theo dõi lịch sử thay đổi.

Tham số lọc/sắp xếp gợi ý (danh sách người dùng)
- `keyword`: tìm theo `userName`, `fullName`, `email`.
- `status`: hoạt động/khóa/pending/require-password-change.
- `role`: mã vai trò.
- `sortBy`, `sortDir`: ví dụ `createdAt` + `desc` hoặc `userName` + `asc`.
- `pageIndex`, `pageSize`.

API Xác thực
- POST `api/auth/login`
  - Đăng nhập, trả về `accessToken`, `refreshToken`, `expiresIn`.
- POST `api/auth/refresh`
  - Làm mới access token từ refresh token hợp lệ.
- POST `api/auth/logout`
  - Đăng xuất và thu hồi refresh token hiện tại.

API Phân quyền & Người dùng liên quan
- GET `api/users/approved`
  - Danh sách người dùng đã duyệt (hỗ trợ phân trang/lọc/sắp xếp để gán quyền nhanh).
- GET `api/users/approved/{userName}`
  - Chi tiết người dùng kèm role/permission hiện tại.
- POST `api/users/{userName}/roles`
  - Cập nhật vai trò cho người dùng (gán/bỏ nhiều role).
- POST `api/users/{userName}/lock`
  - Khóa đăng nhập người dùng.
- DELETE `api/users/{userName}/lock`
  - Mở khóa đăng nhập người dùng.

Gợi ý giao diện
- Màn hình Đăng nhập: form user/password, hiển thị thông báo lockout/đổi mật khẩu.
- Màn hình Danh sách người dùng: bảng cột (UserName, Họ tên, Email, Trạng thái, Vai trò, Ngày tạo) + ô tìm kiếm, bộ lọc vai trò, sắp xếp, phân trang.
- Màn hình Phân quyền: panel gán/bỏ vai trò, xem nhanh quyền hiệu lực; nút Lưu thay đổi.
- Thông báo phiên: hiển thị thời gian còn lại của access token, tự động refresh trước khi hết hạn.

Quy ước & bảo mật
- Lưu audit cho đăng nhập/đăng xuất, thay đổi vai trò/quyền.
- Băm mật khẩu bằng thuật toán mạnh; bật lockout theo số lần sai liên tiếp.
- Refresh token có thể bị thu hồi theo người dùng/thiết bị.
- Áp dụng chính sách mật khẩu mạnh và bắt buộc đổi mật khẩu lần đầu khi cần.

Phần hệ liên quan
- src/Services/Identity/Flex.AspNetIdentity.Api/Controllers/UserController.cs
- src/Services/Identity/Flex.AspNetIdentity.Api/Controllers/RoleController.cs
