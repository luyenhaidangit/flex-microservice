# Luồng Duyệt Thực Thể với Chức Năng Draft/Maker

## 1. Mô tả tổng quan
Luồng này áp dụng cho các thực thể cần phê duyệt (approve) trước khi ghi nhận vào hệ thống. Chức năng **Draft/Maker** cho phép người dùng tạo mới hoặc chỉnh sửa dữ liệu ở trạng thái nháp (Draft), chưa gửi duyệt (Submit).

## 2. Các bước luồng xử lý

### Bước 1: Tạo mới hoặc chỉnh sửa ở trạng thái Draft
- Người dùng (Maker) tạo mới hoặc chỉnh sửa thực thể.
- Dữ liệu được lưu với trạng thái `Draft` (chưa gửi duyệt).
- Maker có thể lưu nhiều lần, chỉnh sửa, xóa bản nháp trước khi submit.

### Bước 2: Gửi duyệt (Submit)
- Khi hoàn tất, Maker thực hiện thao tác gửi duyệt.
- Trạng thái chuyển sang `Pending` (chờ duyệt).
- Không cho phép chỉnh sửa bản ghi khi đã submit.

### Bước 3: Duyệt hoặc từ chối
- Người duyệt (Checker/Approver) xem xét yêu cầu.
- Có thể **Approve** (phê duyệt) hoặc **Reject** (từ chối).
- Nếu duyệt: cập nhật trạng thái thực thể, ghi nhận vào bảng master, ghi log audit.
- Nếu từ chối: trạng thái chuyển về `Rejected`, Maker có thể chỉnh sửa lại và gửi duyệt lại.

### Bước 4: Kết thúc
- Sau khi được duyệt, dữ liệu chính thức ghi nhận vào hệ thống.
- Nếu bị từ chối, Maker có thể tiếp tục chỉnh sửa và gửi lại.

## 3. Lưu ý triển khai
- Sử dụng bảng request header/data để lưu trạng thái Draft/Pending/Rejected.
- Audit log cho mọi thao tác chuyển trạng thái.
- Kiểm soát quyền Maker/Checker rõ ràng.
- Có thể áp dụng cache invalidation nếu dữ liệu master bị thay đổi.

## 4. Ví dụ trạng thái
| Trạng thái   | Ý nghĩa                        |
|--------------|-------------------------------|
| Draft        | Đang nhập, chưa gửi duyệt      |
| Pending      | Đã gửi duyệt, chờ phê duyệt    |
| Approved     | Đã được duyệt, ghi nhận master |
| Rejected     | Bị từ chối, chờ chỉnh sửa lại  |

## 5. Sơ đồ luồng (minh họa)

```mermaid
flowchart TD
    A[Maker tạo/chỉnh sửa Draft] -->|Lưu nháp| A
    A -->|Submit| B[Chuyển trạng thái Pending]
    B --> C{Checker duyệt?}
    C -- Yes --> D[Approved → Ghi nhận master]
    C -- No  --> E[Rejected → Quay lại Maker]
    E -->|Chỉnh sửa lại| A

    ✅ Mình đã xem lại rõ ảnh modal “Chi tiết vai trò (nháp)” của bạn. Đây là **màn read-only xem chi tiết NHÁP**, hiện tại nhìn khá đơn giản và sạch. Sau đây là **phân tích UX/UI + gợi ý nâng cấp cho đúng nghiệp vụ nháp**.

---

## 🚀 Đánh giá hiện trạng

| Yếu tố      | Nhận xét                                                                                                                                                                     |
| ----------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| ➕ Nội dung  | Đang hiển thị các trường cơ bản: Mã vai trò, Tên vai trò, Mô tả, Trạng thái, Người tạo, Ngày tạo.                                                                            |
| ➕ Giao diện | Đơn giản, dễ đọc, đủ space                                                                                                                                                   |
| ➖ Thiếu     | Không nhấn mạnh trạng thái `NHÁP`. Không có action ngay để chỉnh sửa hoặc gửi duyệt từ đây. Không có badge màu trạng thái. Không có note rõ đây chưa phải dữ liệu hoạt động. |

---

## ✅ Đề xuất cải tiến UX/UI

### 🔥 1. Làm nổi bật trạng thái NHÁP

* Ngay tiêu đề:

  ```
  Chi tiết vai trò
  ( 📝 NHÁP )
  ```
* Hoặc thêm badge xám:

  ```
  <span class="badge badge-secondary">NHÁP</span>
  ```

---

### 📝 2. Thêm note nghiệp vụ

* Ở cuối modal, thêm đoạn nhỏ:

  ```
  Đây là bản nháp, chưa được phê duyệt. Bạn có thể chỉnh sửa, lưu tiếp hoặc gửi duyệt.
  ```

---

### ✏ 3. Thêm action trực tiếp

* Đặt ở footer modal:

  ```
  [✏ Sửa nháp] [🚀 Gửi duyệt] [🗑 Xoá nháp] [Đóng]
  ```
* Giúp người dùng thao tác nhanh thay vì phải quay ra list.

---

### 🖥 4. Format ngày & người

* Ngày nên full `03/07/2025 14:35`.
* Người nên thêm tên hiển thị, ví dụ:

  ```
  Người tạo: admin (Nguyễn Văn A)
  ```

---

### 🧩 5. Tooltip cho mô tả dài

* Hover trên mô tả, show tooltip full text nếu dài.

---

## ✅ Tổng quan nghiệp vụ

| Khi xem NHÁP, modal này sẽ cho                   |
| ------------------------------------------------ |
| ✅ Nhấn mạnh đây là NHÁP (badge / tiêu đề).       |
| ✅ Biết đây chưa áp dụng chính thức.              |
| ✅ Có nút: ✏ Sửa / 🚀 Gửi duyệt / 🗑 Xoá.         |
| ✅ Xem thông tin ai tạo, ngày tạo, trạng thái rõ. |

---

## ✍ Kết luận

👉 Hiện modal của bạn **đủ tối thiểu** nhưng chưa tận dụng UX để “hướng nghiệp vụ” rõ.

* ➔ Cần nhấn mạnh `NHÁP`, giải thích đây chỉ là data chuẩn bị, và cho action nhanh.

---

💪 Nếu muốn, mình có thể:
✅ Viết luôn template HTML cải tiến cho modal này,
✅ Hoặc build object JSON hiển thị để bạn bind động cho tất cả trạng thái (`NHÁP`, `CHỜ DUYỆT`, `REJECTED`, `APPROVED`).

Bạn chọn nhé 🚀.

```

- CODE: Nên tự upper, không có dấu cách
- Lưu nháp thường không hỏi confirm, hoặc autosave luôn.
- Nháp - Edit - Save nháp - Submit for Approval - xóa nháp
