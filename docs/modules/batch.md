# Batch Processing

## Định nghĩa
Batch là tập hợp các chương trình chạy tự động theo lịch, xử lý khối lượng dữ liệu lớn không cần tương tác người dùng.

## Đặc điểm chính
- **Scheduled**: Cron job, Task Scheduler, Quartz, Hangfire
- **Bulk processing**: Xử lý hàng triệu bản ghi
- **Automated**: Không cần can thiệp thủ công
- **Periodic**: EOD, EOM, EOQ
- **Non-real-time**: Kết quả có độ trễ

## Nghiệp vụ phổ biến

### Ngân hàng/Tài chính
- **EOD**: Chốt số dư, tính lãi/lỗ, hạch toán phí
- **EOM/EOQ**: Sinh báo cáo tài chính
- **Settlement**: Đối soát giao dịch

### Chức năng sử dụng
- Chốt danh sách cổ đông hưởng quyền
- Thanh toán T+2/T+3
- Tính cổ tức, trái tức
- Đồng bộ dữ liệu (ERP, CRM, DWH)
- Sinh báo cáo bán hàng, tồn kho
- Backup & archive

## Quy trình xử lý
1. **Prepare**: Thu thập, làm sạch dữ liệu
2. **Execute**: Chạy theo thứ tự bước (Step 1 → 2 → 3...)
3. **Error handling**: Log, rollback, retry
4. **Result**: Cập nhật DB, sinh báo cáo
5. **Notification**: Mail/Slack thông báo kết quả

## Yêu cầu kỹ thuật
- **Timeliness**: Đúng hạn (EOD trước ngày mới)
- **Accuracy**: Không mất/sai dữ liệu
- **Restart capability**: Chạy lại từ bước lỗi
- **Monitoring**: Log, dashboard, alerting
- **Performance**: Tối ưu xử lý khối lượng lớn

## Pattern trong Flex System
```csharp
// Background Service Pattern
public class BatchService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessBatch();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
```

## Notes
- **Idempotent**: Chạy nhiều lần không ảnh hưởng kết quả
- **Atomic**: Rollback toàn bộ nếu có lỗi
- **Logging**: Chi tiết từng bước xử lý
- **Monitoring**: Health check, metrics, alerting
- **Error recovery**: Retry với exponential backoff
