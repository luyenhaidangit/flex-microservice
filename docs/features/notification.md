- Chức năng Quản lý Template Notification:
Quản lý Template

UC01 – Tạo mới Template Notification
Maker tạo template mới với các trường:

Mã template (NotificationCode).

Kênh (Email, SMS, App push, In-App).

Tiêu đề (Title/Subject).

Nội dung (Content, hỗ trợ placeholders như {{InvestorName}}, {{StockCode}}).

Ngôn ngữ (nếu đa ngôn ngữ).

Trạng thái (Draft/Inactive).

UC02 – Chỉnh sửa Template
Sửa nội dung/tiêu đề/kênh placeholders (khi chưa Active hoặc bản nháp).

UC03 – Xem chi tiết Template
Xem nội dung, version, lịch sử chỉnh sửa, ai tạo.

UC04 – Kích hoạt/Vô hiệu hóa Template

Active: có thể sử dụng trong hệ thống.

Inactive: ngừng sử dụng nhưng không xóa dữ liệu.

UC05 – Quản lý Version Template
Lưu lịch sử thay đổi, rollback về version cũ.

UC06 – Xóa Template
Chỉ khi chưa từng sử dụng (trạng thái Draft).

2. Quản lý Dynamic Content

UC07 – Định nghĩa & quản lý placeholders
Ví dụ: {{InvestorName}}, {{OrderId}}, {{MatchedPrice}}.

UC08 – Validate placeholders khi lưu
Hệ thống kiểm tra template chỉ dùng placeholders đã được định nghĩa.

3. Quản lý Đa kênh

UC09 – Tạo template cho nhiều kênh
Ví dụ: cùng một sự kiện “Lệnh khớp thành công” → có template SMS, Email, App Push khác nhau.

UC10 – Preview template theo kênh
Cho phép người dùng xem trước hiển thị.

4. Phê duyệt & Quản trị

UC11 – Workflow phê duyệt template

Maker tạo/sửa → Checker duyệt (Approve/Reject).

Audit log ghi lại toàn bộ thao tác.

UC12 – Quản lý phân quyền người dùng

Ai được tạo/sửa.

Ai được duyệt/kích hoạt.

5. Sử dụng & Tích hợp

UC13 – Tra cứu template qua API
Hệ thống giao dịch gọi API lấy template active theo NotificationCode + Ngôn ngữ.

UC14 – Test gửi notification từ template
Gửi thử đến email/số điện thoại test để xem hiển thị thực tế.

6. Quản trị & Báo cáo

UC15 – Tìm kiếm & lọc template
Theo kênh, trạng thái, người tạo, thời gian.

UC16 – Audit log template
Ghi lại: ai tạo, ai sửa, ai duyệt, khi nào.

UC17 – Báo cáo sử dụng template
Thống kê template nào được sử dụng nhiều/ít, kênh nào phổ biến.