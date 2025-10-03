User Management
===============

Tài liệu này mô tả chức năng Quản lý người dùng trong hệ thống Flex Identity theo phong cách hướng dẫn sử dụng: mục tiêu, thao tác chính, và API tham chiếu.

Mục tiêu
- Tạo/sửa/xóa người dùng theo quy trình duyệt (request → approve/reject).
- Tìm kiếm, lọc, sắp xếp người dùng đã duyệt và yêu cầu đang chờ duyệt.
- Quản lý trạng thái tài khoản, buộc đổi mật khẩu lần đầu, và đổi mật khẩu.

Khái niệm chính
- Approved Users: Người dùng đã được duyệt, đang hoạt động trong hệ thống.
- Pending Requests: Yêu cầu tạo/sửa/xóa người dùng, chờ duyệt hoặc bị từ chối.
- Password Change Required: Cờ yêu cầu người dùng đổi mật khẩu ở lần đăng nhập đầu tiên.

Tác vụ thường dùng
- Tìm kiếm người dùng: theo `userName`, họ tên, email, vai trò.
- Lọc trạng thái: đang hoạt động/đã khóa/cần đổi mật khẩu.
- Sắp xếp: theo `createdAt`, `userName`, trạng thái.
- Tạo mới người dùng: gửi yêu cầu tạo, chờ duyệt trước khi có hiệu lực.
- Cập nhật thông tin người dùng: gửi yêu cầu cập nhật và chờ duyệt.
- Xóa/khóa người dùng: gửi yêu cầu xóa hoặc khóa, chờ duyệt.
- Duyệt/Từ chối yêu cầu pending.
- Đổi mật khẩu hoặc kiểm tra bắt buộc đổi mật khẩu lần đầu.

Luồng nghiệp vụ (tóm tắt)
1) Tạo/Sửa/Xóa:
   - Người lập gửi yêu cầu (request) → chuyển vào danh sách Pending.
   - Người có thẩm quyền duyệt hoặc từ chối. Khi duyệt, hệ thống áp dụng vào danh sách Approved Users.
2) Đổi mật khẩu lần đầu:
   - Tài khoản mới có cờ `password-change-required` → khi đăng nhập lần đầu phải đổi mật khẩu.
3) Tìm kiếm & sắp xếp:
   - Hỗ trợ phân trang, từ khóa, lọc, sắp xếp để thao tác nhanh với dữ liệu lớn.

Tham số lọc/sắp xếp gợi ý
- `keyword`: tìm theo `userName`, `fullName`, `email`.
- `status`: hoạt động/khóa/pending.
- `role`: mã vai trò.
- `sortBy`, `sortDir`: ví dụ `createdAt` + `desc`.
- `pageIndex`, `pageSize`.

API Người dùng
- GET `api/users/approved`
  - Lấy danh sách người dùng đã duyệt (phân trang + lọc + sắp xếp).
  - Query: `GetUsersPagingRequest`.
- GET `api/users/approved/{userName}`
  - Lấy chi tiết người dùng theo `userName`.
- GET `api/users/approved/{userName}/history`
  - Xem lịch sử thay đổi người dùng.
- GET `api/users/request/pending`
  - Danh sách yêu cầu người dùng đang chờ duyệt (phân trang).
- GET `api/users/request/pending/{requestId}`
  - Chi tiết yêu cầu người dùng cụ thể.
- POST `api/users/request/create`
  - Tạo yêu cầu tạo mới người dùng.
- POST `api/users/request/update`
  - Tạo yêu cầu cập nhật người dùng.
- POST `api/users/request/delete/{userName}`
  - Tạo yêu cầu xóa người dùng.
- POST `api/users/request/pending/{requestId}/approve`
  - Phê duyệt yêu cầu.
- POST `api/users/request/pending/{requestId}/reject`
  - Từ chối yêu cầu, kèm lý do.
- POST `api/users/change-password`
  - Đổi mật khẩu người dùng.
- GET `api/users/{userName}/password-change-required`
  - Kiểm tra cờ bắt buộc đổi mật khẩu lần đầu.

Gợi ý giao diện
- Danh sách người dùng: bảng cột (UserName, Họ tên, Email, Trạng thái, Vai trò, Ngày tạo), có ô tìm kiếm, bộ lọc, sắp xếp, phân trang.
- Form tạo/sửa: thông tin cơ bản + chọn vai trò + ghi chú; submit tạo “yêu cầu”.
- Danh sách yêu cầu: trạng thái, người tạo, thời điểm, lý do; nút Duyệt/Từ chối.
- Chi tiết người dùng: tab Lịch sử thay đổi, hiển thị vai trò/quyền hiệu lực.

Quy ước & bảo mật
- Thay đổi dữ liệu người dùng luôn đi qua quy trình request → approve để đảm bảo kiểm soát.
- Lưu lịch sử thay đổi, log audit.
- Phân quyền theo vai trò; chỉ người có quyền mới được duyệt.
- Áp dụng chính sách độ mạnh mật khẩu và bắt buộc đổi mật khẩu lần đầu khi cần.

Phân hệ liên quan
- src/Services/Identity/Flex.AspNetIdentity.Api/Controllers/UserController.cs
