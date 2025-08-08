# Nghiệp vụ Quản lý Chi nhánh

## 1. Khái niệm & Vai trò
- Chi nhánh là đơn vị trực thuộc của ngân hàng lưu ký.
- Thực hiện dịch vụ lưu ký chứng khoán, thanh toán, quản lý tài khoản cho khách hàng.
- Là điểm giao dịch chính tại từng khu vực.

## 2. Đối tượng quản lý
- **Mã chi nhánh (Code)** – duy nhất.
- **Tên chi nhánh (Name)** – tên giao dịch.
- **Mô tả (Descrition)**
- **Trạng thái hoạt động (IsActive)**: true/false.
- **Ngày thành lập / Ngày ngừng hoạt động**.
- **Người quản lý chính (Branch Manager)**.
- **Thông tin liên hệ**: số điện thoại, email.

## 3. Nghiệp vụ chính

### 3.1. Tạo mới chi nhánh
- Maker nhập thông tin chi nhánh mới.
- Lưu yêu cầu ở trạng thái **Pending**.
- Checker phê duyệt hoặc từ chối.
- Sau khi duyệt:
  - Kích hoạt chi nhánh trong hệ thống.

### 3.2. Cập nhật thông tin chi nhánh
- Thay đổi địa chỉ, tên, người quản lý, trạng thái.
- Yêu cầu duyệt giống quy trình tạo mới.
- Lưu lịch sử thay đổi.

### 3.3. Ngừng hoạt động / Đóng chi nhánh
- Maker gửi yêu cầu đóng.
- Checker phê duyệt → trạng thái **false**.
- Thực hiện chuyển giao khách hàng (nếu cần).

### 3.4. Tra cứu & Báo cáo
- Tìm kiếm theo mã, tên, địa chỉ, trạng thái.
- Báo cáo danh sách chi nhánh đang hoạt động.
- Lịch sử thay đổi chi nhánh.

## 4. Quy trình duyệt (Maker / Checker)
- Mọi thay đổi phải qua **2 lớp xác thực**.
- Quy trình:
  1. Maker tạo hoặc chỉnh sửa dữ liệu.
  2. Lưu vào bảng `BRANCH_REQUESTS` trạng thái **Pending**.
  3. Checker duyệt:
     - Nếu **Approve** → cập nhật bảng `BRANCHES`, đồng bộ hệ thống, ghi Audit log.
     - Nếu **Reject** → cập nhật trạng thái yêu cầu, ghi Audit log.

## 5. Sơ đồ luồng quy trình

[Maker]
│
▼
[Nhập thông tin] → [Lưu vào BRANCH_REQUESTS: Pending]
│
▼
[Checker]
│
├── Approve → [Cập nhật BRANCHES] → [Đồng bộ hệ thống] → [Audit log]
│
└── Reject → [Cập nhật trạng thái Rejected] → [Audit log]

## 7. Mối liên hệ với hệ thống khác
- **Quản lý Role & Permission**: kiểm soát ai được thao tác.
- **Quản lý Tài khoản lưu ký**: xử lý khi đóng chi nhánh.
- **Core Banking / VSD**: đồng bộ mã và trạng thái chi nhánh.