# API Xem Chi Tiết Role Request 🚀

## 📋 Mô tả
API để lấy thông tin chi tiết request (vai trò đang chờ duyệt) để hiển thị trong modal.

## 🔍 Endpoint
```
GET /api/roles/role-requests/{requestId}
```

## 🔄 Response JSON

### Ví dụ cho CREATE request:
```json
{
  "success": true,
  "data": {
    "requestId": "123",
    "type": "CREATE",
    "createdBy": "admin",
    "createdDate": "2025-01-12",
    "oldData": null,
    "newData": {
      "roleCode": "ADMIN",
      "roleName": "Administrator",
      "description": "Quản trị hệ thống",
      "permissions": ["ViewUser", "EditUser"]
    }
  }
}
```

### Ví dụ cho UPDATE request:
```json
{
  "success": true,
  "data": {
    "requestId": "124",
    "type": "UPDATE",
    "createdBy": "admin",
    "createdDate": "2025-01-12",
    "oldData": {
      "roleCode": "ADMIN",
      "roleName": "Administrator",
      "description": "Quản trị hệ thống",
      "permissions": ["ViewUser", "EditUser"]
    },
    "newData": {
      "roleCode": "ADMIN",
      "roleName": "Admin",
      "description": "Quản lý hệ thống & user",
      "permissions": ["ViewUser", "EditUser", "DeleteUser"]
    }
  }
}
```

### Ví dụ cho DELETE request:
```json
{
  "success": true,
  "data": {
    "requestId": "125",
    "type": "DELETE",
    "createdBy": "admin",
    "createdDate": "2025-01-12",
    "oldData": {
      "roleCode": "GUEST",
      "roleName": "Guest User",
      "description": "Người dùng khách",
      "permissions": ["ViewPublic"]
    },
    "newData": null
  }
}
```

## 📝 Các trường dữ liệu

### RoleRequestDetailDto
- `requestId`: ID của request (string)
- `type`: Loại request - CREATE/UPDATE/DELETE (string)
- `createdBy`: Người tạo request (string)
- `createdDate`: Ngày tạo request (string, format: yyyy-MM-dd)
- `oldData`: Dữ liệu cũ (null cho CREATE, có giá trị cho UPDATE/DELETE)
- `newData`: Dữ liệu mới (null cho DELETE, có giá trị cho CREATE/UPDATE)

### RoleDetailDataDto
- `roleCode`: Mã vai trò (string)
- `roleName`: Tên vai trò (string)
- `description`: Mô tả vai trò (string)
- `permissions`: Danh sách quyền (List<string>)

## 🎯 Cách sử dụng

### Frontend (JavaScript/TypeScript)
```javascript
// Gọi API khi user click nút xem chi tiết
async function viewRequestDetail(requestId) {
  try {
    const response = await fetch(`/api/roles/role-requests/${requestId}`, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    });
    
    const result = await response.json();
    
    if (result.success) {
      const data = result.data;
      
      // Hiển thị modal với thông tin
      showDetailModal(data);
    } else {
      console.error('Failed to get request detail:', result.message);
    }
  } catch (error) {
    console.error('Error fetching request detail:', error);
  }
}

function showDetailModal(data) {
  // Hiển thị thông tin cơ bản
  document.getElementById('requestId').textContent = data.requestId;
  document.getElementById('requestType').textContent = data.type;
  document.getElementById('createdBy').textContent = data.createdBy;
  document.getElementById('createdDate').textContent = data.createdDate;
  
  // Hiển thị dữ liệu cũ và mới
  if (data.oldData) {
    displayRoleData('oldData', data.oldData);
  }
  
  if (data.newData) {
    displayRoleData('newData', data.newData);
  }
  
  // Mở modal
  $('#detailModal').modal('show');
}
```

## ⚠️ Lưu ý
- API chỉ trả về dữ liệu cho các request có trạng thái PENDING hoặc DRAFT
- Với CREATE request: chỉ có `newData`
- Với UPDATE request: có cả `oldData` và `newData`
- Với DELETE request: chỉ có `oldData`
- Permissions được format dưới dạng "Type:Value" (ví dụ: "ViewUser", "EditUser:Create") 