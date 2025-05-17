# Hướng dẫn cập nhật Oracle Wallet

## 1. Chuẩn bị

- Tải Oracle Wallet từ Oracle Cloud Console
- Giải nén file zip chứa wallet

## 2. Cấu trúc thư mục wallet

```
flex-oracle-wallet/
├── cwallet.sso
├── ewallet.p12 
├── keystore.jks
├── ojdbc.properties
├── sqlnet.ora
├── tnsnames.ora
└── truststore.jks
```

## 3. Các bước cập nhật

1. Tạo backup của wallet hiện tại:
```bash
cp -r flex-oracle-wallet flex-oracle-wallet.bak
```

2. Copy các file wallet mới vào thư mục:
```bash
cp /path/to/new/wallet/* flex-oracle-wallet/
```

3. Kiểm tra quyền truy cập:
```bash
chmod 600 flex-oracle-wallet/*
```

## 4. Cấu hình trong ứng dụng

Thêm các thông tin sau vào file appsettings.json:

```json
{
  "OracleWallet": {
    "WalletLocation": "/path/to/flex-oracle-wallet",
    "TNSAdmin": "/path/to/flex-oracle-wallet"
  }
}
```

## 5. Kiểm tra kết nối 

Sử dụng lệnh sau để kiểm tra kết nối:

```bash
sqlplus username/password@db_name
```

## 6. Xử lý sự cố

- Kiểm tra quyền truy cập file
- Xác nhận đường dẫn wallet trong cấu hình
- Đảm bảo TNS_ADMIN được set đúng
- Kiểm tra log ứng dụng

## 7. Lưu ý bảo mật

- Không commit wallet vào source control
- Giữ file backup an toàn
- Hạn chế quyền truy cập thư mục wallet
- Thay đổi mật khẩu wallet định kỳ

## 8. Tham khảo

- [Oracle Wallet Documentation](https://docs.oracle.com/en/database/oracle/oracle-database/19/dbimi/installing-oracle-wallet.html)
- [Oracle Cloud Console Guide](https://docs.oracle.com/en-us/iaas/Content/Database/Tasks/managingwallets.htm)
