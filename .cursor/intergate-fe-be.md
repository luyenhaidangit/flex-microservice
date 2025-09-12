# Integration Frontend - Backend Checklist

## 1. Model Synchronization

### Mục tiêu
Đảm bảo model request giữa Backend và Frontend đồng bộ hoàn toàn, tránh lỗi validation và data mismatch.

### Các bước thực hiện
- **Audit Model Fields**: So sánh chi tiết từng field trong request model giữa FE và BE
- **Identify Discrepancies**: Xác định fields thừa/thiếu giữa FE và BE
- **Standardize Required Fields**: Thống nhất danh sách fields bắt buộc
- **Update Validators**: Cập nhật validation rules cho phù hợp
- **Update Services**: Điều chỉnh business logic nếu cần

### Convention áp dụng
- **DTO/ViewModel**: `string.Empty` cho string properties
- **Entity**: `default!` cho string properties  
- **Optional fields**: `string?` và không khởi tạo

### Đề xuất phương án
1. **Xóa fields thừa**: Loại bỏ fields không cần thiết ở FE hoặc BE
2. **Thêm fields thiếu**: Bổ sung fields cần thiết cho đầy đủ chức năng
3. **Thống nhất required fields**: Đảm bảo validation rules nhất quán
4. **Cập nhật validator và service**: Đồng bộ logic xử lý

## 2. Frontend UI Validation

### Mục tiêu
Tối ưu hóa CSS và UI components theo chuẩn dự án, đảm bảo tính nhất quán và performance.

### 2.1 CSS Architecture Review
- **Audit Component Styles**: Kiểm tra các file CSS riêng lẻ của components
- **Check Core Classes**: Xác định classes đã có trong `core.scss`, `core_table.scss`
- **Remove Duplicates**: Xóa các style trùng lặp, sử dụng lại core classes
- **Standardize Imports**: 
  - Màn hình chính → import `core_table`
  - Modal components → import `core_modal`

### 2.2 Input State Management
- **Apply Disabled Styles**: Sử dụng core classes cho input disabled
- **Visual Feedback**: Đảm bảo màu xám khi disable để nhận biết
- **Consistent UX**: Thống nhất trải nghiệm người dùng

### Core Classes cần sử dụng
```scss
// Input states
.input-disabled { /* màu xám khi disable */ }
.input-readonly { /* style cho readonly fields */ }
.input-error { /* style cho validation error */ }

// Table styles
.table-core { /* base table styles */ }
.table-responsive { /* responsive table */ }

// Modal styles  
.modal-core { /* base modal styles */ }
.modal-header { /* modal header */ }
```

## 3. Implementation Priority

1. **High Priority**: Model synchronization (ảnh hưởng đến data flow)
2. **Medium Priority**: CSS optimization (ảnh hưởng đến performance)
3. **Low Priority**: UI polish (ảnh hưởng đến UX)

## 4. Success Criteria

- Tất cả request models đồng bộ giữa FE và BE
- Không có CSS duplicate, sử dụng core classes
- UI states (disabled, readonly) hiển thị nhất quán
- Performance cải thiện (giảm CSS bundle size)