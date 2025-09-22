# Batch

## Nghiệp vụ
- Batch là tập hợp các chương trình chạy tự động theo lịch, được thực hiện ở những mốc thời gian nhất định, phụ thuộc vào lịch vận hành của hệ thống.
- Batch thực hiện các giao dịch theo lô, không cần thao tác thủ công của người dùng, chạy theo chu kỳ (daily, monthly, EOD/SOD, T+1…) để thực hiện các công việc sau:
  + Tự động hóa xử lý khối lượng lớn.
  + Kết sổ và chuẩn bị sổ sách.
  + Đối chiếu và kiểm tra toàn vẹn.
  + Kết xuất và truyền thông tin.
  + Lưu trữ, backup và audit.
  + Đảm bảo an toàn và tính liên tục.
## Chu kỳ vận hành
- Cuối ngày giao dịch (EOD – End of Day) (quan trọng)
  + Thời điểm: Sau khi ngân hàng ngừng nhận giao dịch trong ngày (cut-off, thường 17h–20h cho từng loại giao dịch).
  + Mục đích:
    - Khóa ngày (business_date) → không còn nhận thêm booking T0.
    - Thực hiện xử lý batch chính: tính toán, kết sổ, đối chiếu, tạo báo cáo, gửi điện/file outbound.
    - Đây là batch quan trọng nhất, thường kéo dài vài tiếng (ví dụ 20h–00h).
- Đầu ngày giao dịch (SOD – Start of Day)
  + Thời điểm: Rạng sáng hôm sau (thường 00h30–03h00).
  + Mục đích:
    - Khởi tạo dữ liệu & tham số cho ngày mới.
    - Nạp lịch nghỉ, tỷ giá, hạn mức.
    - Mở sổ, cho phép bắt đầu ghi nhận giao dịch T+1.
- Intraday Batch (giữa ngày)
  + Thời điểm: Nhiều lần trong ngày, theo phiên clearing hoặc theo cut-off nội bộ (ví dụ 11h, 15h).
  + Mục đích:
  + Đối chiếu tạm thời (clearing, Nostro).
    - Chạy batch xử lý lô cho một số nghiệp vụ (ví dụ: card settlement, ACH theo phiên).
    - Thường chạy nhanh, không ảnh hưởng toàn bộ hệ thống, nhưng cần để giữ cân bằng trong ngày.
- Batch định kỳ (Monthly/Quarterly/Yearly)
  + Thời điểm: Cuối tháng, cuối quý, cuối năm.
  + Mục đích:
    - Kết chuyển, khấu hao, phân bổ chi phí.
    - Chạy báo cáo quản lý & regulator (Basel, IFRS, NHNN…).
    - Được tích hợp chung vào lịch batch, nhưng thời gian chạy thường dài hơn EOD.
## Luồng vận hành bước batch
- Được vận hành bởi scheduler hệ thống, giám sát bởi đội Ops/IT, và có sự tham gia gián tiếp của nghiệp vụ & kiểm toán. Nó chạy theo một luồng chặt chẽ: khởi chạy → kiểm tra đầu vào → xử lý → đối chiếu → kết xuất → đóng ngày → backup → giám sát.
1. Trigger (khởi chạy)
- Batch được kích hoạt theo lịch (ví dụ 20h00 chạy EOD, 01h00 chạy SOD).
- Có thể được khởi động bằng tay (manual trigger) trong trường hợp đặc biệt.
2. Pre-check
- Kiểm tra điều kiện đầu vào: đã hết giao dịch? file inbound đầy đủ? cut-off đã tới?
- Nếu không đạt điều kiện, batch sẽ bị hoãn hoặc gửi cảnh báo.
3. Staging & Validation
- Tập hợp dữ liệu vào khu vực trung gian.
- Giải mã/giải nén, kiểm tra tổng số bản ghi, tổng tiền (control totals).
4. Processing (xử lý chính)
- Thực thi các bước theo DAG (Directed Acyclic Graph) – job này phụ thuộc job kia.
- Ví dụ: job A (tính lãi) phải xong trước job B (ghi sổ).
- Có checkpoint để nếu lỗi có thể chạy lại từ điểm dừng.
5. Reconciliation (đối chiếu)
- So sánh số liệu giữa các hệ thống/sổ.
- Nếu lệch → tạo log, gửi cảnh báo, có thể sinh adjustment.
6, Output/Export
- Sinh báo cáo, file kết quả, điện outbound.
- Ký số, mã hóa, gửi qua kênh chính thức (SFTP, SWIFT, API).
Close/Finalize
7. Đóng ngày giao dịch, cập nhật business_date.
- Backup, snapshot dữ liệu.
- Ghi log kết quả, gửi thông báo batch success/fail.
- Đóng giao dịch ngày → khoanh business_date (không nhận thêm booking T0).
8. Monitoring & Alerting
- Kết quả batch được giám sát qua dashboard.
- Nếu quá SLA hoặc có lỗi → gửi cảnh báo cho Ops/IT và đội nghiệp vụ.
## Ví dụ luồng

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
- Sinh báo cáo bán hàng, tồn kho.
- Backup & archive.

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

## Lưu ý triển khai

### Thời gian chạy batch
- **Scheduled time**: Cấu hình thời gian chạy batch (cron expression)
- **Business day**: Chỉ chạy trong ngày làm việc (trừ T7, CN, lễ)
- **Time window**: Batch phải hoàn thành trong khung giờ cho phép

### Tham số chạy batch
- **A (Auto)**: Chạy tự động theo lịch
- **M (Manual)**: Chạy thủ công bởi admin
- **Mutual exclusion**: Auto và Manual không được chạy đồng thời
- **Lock mechanism**: Chặn batch mới khi đang có batch đang chạy

### Cơ chế chặn batch
```csharp
// Batch Lock Pattern
public class BatchLockService
{
    private readonly IDistributedLock _lock;
    
    public async Task<bool> TryAcquireLock(string batchName)
    {
        return await _lock.TryAcquireAsync($"batch:{batchName}", TimeSpan.FromHours(2));
    }
    
    public async Task ReleaseLock(string batchName)
    {
        await _lock.ReleaseAsync($"batch:{batchName}");
    }
}
```

### Màn hình tra cứu trạng thái
- **Batch Status Dashboard**: Hiển thị trạng thái real-time
- **User tracking**: Ghi nhận user thực hiện batch
- **Execution time**: Thời gian bắt đầu/kết thúc batch
- **Log viewer**: Xem chi tiết log từng bước

### Quy định chạy batch

#### Quy định 1: Ngày thực tế = Ngày làm việc
- Batch chạy theo ngày làm việc (business day)
- Bỏ qua ngày nghỉ, lễ tết
- Cấu hình calendar trong hệ thống

#### Quy định 2: CHECK_LOCAL_DATE
- **CHECK_LOCAL_DATE = Y**: Batch chỉ chạy khi ngày thực tế = ngày hiện tại
- **CHECK_LOCAL_DATE = N**: Không check điều kiện ngày
- **Lưu ý**: Có thể phát sinh batch 2 lần nếu thay đổi tham số giữa chừng

```csharp
// Date Check Logic
public bool ShouldRunBatch(DateTime currentDate, bool checkLocalDate)
{
    if (!checkLocalDate) return true;
    
    var businessDate = GetBusinessDate(currentDate);
    return businessDate.Date == currentDate.Date;
}
```

### Monitoring & Control
- **TCB Monitor**: Hệ thống giám sát batch job
- **Health check**: Kiểm tra batch có chạy đúng thời điểm
- **Alerting**: Cảnh báo khi batch fail hoặc delay
- **Metrics**: Thống kê performance, success rate

## Notes
- **Idempotent**: Chạy nhiều lần không ảnh hưởng kết quả
- **Atomic**: Rollback toàn bộ nếu có lỗi
- **Logging**: Chi tiết từng bước xử lý
- **Monitoring**: Health check, metrics, alerting
- **Error recovery**: Retry với exponential backoff
