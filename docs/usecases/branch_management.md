# Nghiệp vụ Quản lý Chi nhánh

## 1. Khái niệm & Vai trò
- Chi nhánh là đơn vị trực thuộc của ngân hàng lưu ký.
- Thực hiện dịch vụ lưu ký chứng khoán, thanh toán, quản lý tài khoản cho khách hàng tại từng khu vực.
- Là điểm giao dịch chính phục vụ khách hàng tổ chức và cá nhân.

## 2. Đối tượng quản lý (theo entity Branch)

### 2.1. Các trường đã triển khai
- **Code (Mã chi nhánh)** – duy nhất, phân biệt giữa các chi nhánh.
- **Name (Tên chi nhánh)** – tên giao dịch chính thức.
- **BranchType (Loại chi nhánh)** – ví dụ: Hội sở, Chi nhánh, Phòng giao dịch.
- **IsActive (Trạng thái hoạt động)** – true = đang hoạt động, false = ngừng hoạt động.
- **Status (Trạng thái phê duyệt)** – Pending / Approved / Rejected.
- **Description (Mô tả)** – thông tin bổ sung về chi nhánh.

### 2.2. Các trường chưa triển khai
- **Ngày thành lập / Ngày ngừng hoạt động**.
- **Người quản lý chính (Branch Manager)**.
- **Địa chỉ, tỉnh/thành, số điện thoại, email liên hệ**.
- **Thông tin pháp lý**: số giấy phép, ngày cấp, mã số thuế.

## 3. Nghiệp vụ chính

### 3.1. Tạo mới chi nhánh
- Maker nhập thông tin chi nhánh mới (Code, Name, BranchType, IsActive, Description).
- Lưu yêu cầu vào bảng `BRANCH_REQUESTS` trạng thái **Pending**.
- Checker xem xét và:
  - **Approve** → tạo bản ghi trong `BRANCHES` với Status = Approved.
  - **Reject** → cập nhật trạng thái yêu cầu thành Rejected, ghi lý do.

### 3.2. Cập nhật thông tin chi nhánh
- Maker thay đổi thông tin (Name, BranchType, IsActive, Description).
- Lưu yêu cầu vào `BRANCH_REQUESTS` trạng thái **Pending**.
- Checker duyệt hoặc từ chối.
- **Chưa triển khai**: lưu lịch sử thay đổi chi tiết từng trường.

### 3.3. Ngừng hoạt động / Đóng chi nhánh
- Maker gửi yêu cầu đóng chi nhánh (IsActive = false).
- Checker duyệt → cập nhật trạng thái IsActive = false trong `BRANCHES`.
- **Chưa triển khai**: quy trình chuyển giao khách hàng và tài sản.

### 3.4. Tra cứu & Báo cáo
- Tìm kiếm chi nhánh theo: Code, Name, BranchType, IsActive, Status.
- Xuất báo cáo danh sách chi nhánh đang hoạt động.
- **Chưa triển khai**: lịch sử thay đổi và báo cáo chi tiết.

## 4. Quy trình duyệt (Maker / Checker)
- Tất cả thay đổi phải qua **2 lớp phê duyệt**.
- **Luồng xử lý**:
  1. Maker tạo hoặc chỉnh sửa → lưu vào `BRANCH_REQUESTS` (Pending).
  2. Checker duyệt:
     - **Approve** → ghi vào `BRANCHES`, đồng bộ sang hệ thống khác, log audit.
     - **Reject** → cập nhật trạng thái yêu cầu, log audit.

## 5. Sơ đồ luồng quy trình

```
[Maker]
   │
   ▼
[Nhập thông tin] → [BRANCH_REQUESTS: Pending]
   │
   ▼
[Checker]
   ├── Approve → [Cập nhật BRANCHES] → [Đồng bộ hệ thống] → [Audit log]
   └── Reject  → [Cập nhật Rejected] → [Audit log]
```

## 6. Tích hợp hệ thống
- **Hệ thống phân quyền (Role & Permission)**: chỉ user có quyền mới được tạo/cập nhật/duyệt.
- **Quản lý tài khoản lưu ký**: xử lý khách hàng khi chi nhánh đóng.
- **Core Banking / VSD**: đồng bộ mã chi nhánh và trạng thái hoạt động.

## 7. Cấu trúc Database

### 7.1. Bảng BRANCHES
```sql
CREATE TABLE BRANCHES (
    Id NUMBER PRIMARY KEY,
    Code VARCHAR2(50) UNIQUE NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    BranchType VARCHAR2(50) NOT NULL,
    Description VARCHAR2(500),
    IsActive NUMBER(1) DEFAULT 1,
    Status VARCHAR2(20) DEFAULT 'APPROVED',
    CreatedBy VARCHAR2(100),
    CreatedDate DATE DEFAULT SYSDATE,
    UpdatedBy VARCHAR2(100),
    UpdatedDate DATE
);
```

### 7.2. Bảng BRANCH_REQUESTS
```sql
CREATE TABLE BRANCH_REQUESTS (
    Id NUMBER PRIMARY KEY,
    Action VARCHAR2(20) NOT NULL, -- CREATE, UPDATE, DELETE
    EntityId NUMBER, -- ID của branch (0 cho CREATE)
    EntityCode VARCHAR2(50) NOT NULL,
    Status VARCHAR2(20) DEFAULT 'UNAUTHORISED',
    RequestData CLOB NOT NULL, -- JSON data
    OriginalData CLOB, -- JSON data cho UPDATE/DELETE
    CreatedBy VARCHAR2(100),
    CreatedDate DATE DEFAULT SYSDATE,
    CheckerId VARCHAR2(100),
    ApproveDate DATE,
    Comments VARCHAR2(500)
);
```

## 8. API Endpoints

### 8.1. Quản lý chi nhánh đã duyệt
- `GET /api/branch` - Lấy danh sách chi nhánh đã duyệt (có phân trang)
- `GET /api/branch/{code}` - Lấy thông tin chi nhánh theo mã
- `POST /api/branch` - Tạo yêu cầu tạo mới chi nhánh
- `PUT /api/branch/{code}` - Tạo yêu cầu cập nhật chi nhánh
- `DELETE /api/branch/{code}` - Tạo yêu cầu xóa chi nhánh

### 8.2. Quản lý yêu cầu chờ duyệt
- `GET /api/branch/pending` - Lấy danh sách yêu cầu chờ duyệt
- `GET /api/branch/pending/{requestId}` - Lấy chi tiết yêu cầu chờ duyệt
- `POST /api/branch/pending/{requestId}/approve` - Duyệt yêu cầu
- `POST /api/branch/pending/{requestId}/reject` - Từ chối yêu cầu

## 9. Quyền hạn (Permissions)
- `BRANCH_VIEW` - Xem danh sách chi nhánh
- `BRANCH_CREATE` - Tạo yêu cầu tạo mới chi nhánh
- `BRANCH_UPDATE` - Tạo yêu cầu cập nhật chi nhánh
- `BRANCH_DELETE` - Tạo yêu cầu xóa chi nhánh
- `BRANCH_APPROVE` - Duyệt yêu cầu chi nhánh
- `BRANCH_REJECT` - Từ chối yêu cầu chi nhánh