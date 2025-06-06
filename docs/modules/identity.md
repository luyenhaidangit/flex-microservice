Dịch vụ Xác thực Danh tính (Identity Service) cho Hệ thống Quản lý Giao dịch Chứng khoán
Mục tiêu nghiệp vụ
Dịch vụ xác thực danh tính (Identity Service) đảm bảo quản lý, xác minh và bảo mật danh tính của nhân viên hệ thống chứng khoán, hỗ trợ vận hành giao dịch chứng khoán an toàn và hiệu quả. Dịch vụ tập trung vào các nghiệp vụ quản lý tài khoản, xác thực, phân quyền và giám sát.
Đối tượng sử dụng

Nhân viên hệ thống chứng khoán: Giao dịch viên, nhân viên quản lý tài khoản, nhân viên giám sát thị trường, quản trị viên hệ thống.
Yêu cầu: Hiểu quy trình giao dịch chứng khoán và bảo mật danh tính.

Các chức năng và use case nghiệp vụ
1. Chức năng cơ bản
1.1. Quản lý tài khoản nhân viên

Use case 1.1.1: Tạo tài khoản nhân viên
Diễn viên: Quản trị viên.
Mô tả: Khởi tạo tài khoản cho nhân viên mới để truy cập hệ thống.
Bước thực hiện:
Nhập thông tin: mã nhân viên, email công ty, họ tên, phòng ban.
Xác minh danh tính qua CMND/CCCD hoặc hợp đồng lao động.
Gán vai trò mặc định dựa trên chức năng công việc (ví dụ: giao dịch viên).
Gửi thông báo tài khoản và hướng dẫn thiết lập mật khẩu qua email công ty.


Kết quả: Tài khoản nhân viên được tạo, trạng thái "Hoạt động".


Use case 1.1.2: Cập nhật thông tin nhân viên
Diễn viên: Quản trị viên hoặc nhân viên.
Mô tả: Cập nhật thông tin cá nhân hoặc phòng ban của nhân viên.
Bước thực hiện:
Truy cập hồ sơ nhân viên trong hệ thống.
Cập nhật thông tin: email công ty, số điện thoại, phòng ban.
Ghi lại lịch sử thay đổi với thời gian và người thực hiện.


Kết quả: Thông tin được cập nhật, lịch sử thay đổi được lưu trữ.


Use case 1.1.3: Khóa/Mở tài khoản
Diễn viên: Quản trị viên.
Mô tả: Tạm khóa hoặc mở lại tài khoản nhân viên.
Bước thực hiện:
Chọn tài khoản từ danh sách nhân viên.
Thực hiện hành động: khóa (do nghỉ việc, vi phạm) hoặc mở.
Ghi lại hành động và lý do trong nhật ký.


Kết quả: Tài khoản chuyển trạng thái "Khóa" hoặc "Hoạt động".


Use case 1.1.4: Đặt lại mật khẩu
Diễn viên: Quản trị viên hoặc nhân viên.
Mô tả: Đặt lại mật khẩu khi quên hoặc theo yêu cầu bảo mật.
Bước thực hiện:
Gửi yêu cầu đặt lại mật khẩu qua email công ty.
Xác minh danh tính qua mã OTP gửi qua email.
Nhập mật khẩu mới tuân thủ chính sách: tối thiểu 8 ký tự, bao gồm chữ hoa, chữ thường, số, ký tự đặc biệt.


Kết quả: Mật khẩu được cập nhật, thông báo xác nhận gửi đến nhân viên.



1.2. Xác thực đăng nhập

Use case 1.2.1: Đăng nhập hệ thống
Diễn viên: Nhân viên.
Mô tả: Đăng nhập để thực hiện các nghiệp vụ giao dịch hoặc quản lý.
Bước thực hiện:
Nhập mã nhân viên hoặc email công ty và mật khẩu.
Xác thực hai yếu tố (2FA) qua mã OTP (SMS hoặc ứng dụng xác thực).
Kiểm tra quyền truy cập dựa trên vai trò được gán.


Kết quả: Đăng nhập thành công, truy cập giao diện nghiệp vụ tương ứng.


Use case 1.2.2: Đăng xuất
Diễn viên: Nhân viên.
Mô tả: Đăng xuất để kết thúc phiên làm việc.
Bước thực hiện:
Nhấn nút đăng xuất hoặc hệ thống tự động đăng xuất sau 15 phút không hoạt động.
Hủy phiên đăng nhập hiện tại.


Kết quả: Phiên đăng nhập kết thúc, chuyển về trang đăng nhập.


Use case 1.2.3: Xác minh sinh trắc học
Diễn viên: Quản trị viên hoặc nhân viên giám sát.
Mô tả: Xác thực bổ sung cho các vai trò nhạy cảm.
Bước thực hiện:
Quét vân tay hoặc nhận diện khuôn mặt qua thiết bị được chỉ định.
So khớp với dữ liệu sinh trắc học đã lưu trong hệ thống.


Kết quả: Xác thực thành công, cấp quyền truy cập chức năng nhạy cảm.



1.3. Phân quyền truy cập

Use case 1.3.1: Gán vai trò nhân viên
Diễn viên: Quản trị viên.
Mô tả: Gán vai trò để kiểm soát quyền truy cập của nhân viên.
Bước thực hiện:
Chọn tài khoản nhân viên từ danh sách.
Gán vai trò: giao dịch viên, giám sát thị trường, quản trị viên, v.v.
Áp dụng quyền truy cập theo mô hình RBAC (ví dụ: giao dịch viên chỉ đặt lệnh, quản trị viên quản lý tài khoản).
Ghi lại thay đổi vai trò trong nhật ký.


Kết quả: Vai trò được cập nhật, quyền truy cập được áp dụng.


Use case 1.3.2: Kiểm tra quyền truy cập
Diễn viên: Hệ thống.
Mô tả: Kiểm tra quyền khi nhân viên thực hiện thao tác nghiệp vụ.
Bước thực hiện:
Nhận yêu cầu truy cập chức năng (đặt lệnh, xem báo cáo, quản lý tài khoản).
Kiểm tra vai trò và quyền của nhân viên.
Cho phép hoặc từ chối thao tác.


Kết quả: Thao tác được thực hiện hoặc báo lỗi "Không có quyền".



2. Chức năng nâng cao
2.1. Bảo mật danh tính

Use case 2.1.1: Phát hiện truy cập bất thường
Diễn viên: Hệ thống, quản trị viên.
Mô tả: Phát hiện và xử lý các hành vi đăng nhập bất thường.
Bước thực hiện:
Theo dõi số lần đăng nhập thất bại (khóa sau 5 lần sai).
Phát hiện đăng nhập từ thiết bị hoặc mạng không xác định (ngoài mạng nội bộ).
Gửi cảnh báo qua email công ty và tạm khóa tài khoản.
Quản trị viên xác minh và mở khóa nếu hợp lệ.


Kết quả: Tài khoản được bảo vệ, sự kiện được ghi vào nhật ký.


Use case 2.1.2: Quản lý phiên đăng nhập
Diễn viên: Nhân viên, quản trị viên.
Mô tả: Theo dõi và hủy các phiên đăng nhập để đảm bảo bảo mật.
Bước thực hiện:
Hiển thị danh sách phiên đăng nhập (thiết bị, thời gian, địa chỉ IP).
Cho phép nhân viên hoặc quản trị viên hủy phiên từ xa.


Kết quả: Phiên không cần thiết bị hủy, tăng cường bảo mật.



2.2. Tích hợp hệ thống nội bộ

Use case 2.2.1: Đồng bộ dữ liệu từ hệ thống nhân sự
Diễn viên: Hệ thống.
Mô tả: Cập nhật thông tin nhân viên từ hệ thống HRM.
Bước thực hiện:
Kết nối với hệ thống HRM để lấy dữ liệu: trạng thái việc làm, phòng ban, vai trò.
Cập nhật thông tin tài khoản nhân viên.
Tự động vô hiệu hóa tài khoản của nhân viên nghỉ việc.


Kết quả: Thông tin nhân viên luôn chính xác, tài khoản được quản lý tự động.


Use case 2.2.2: Đăng nhập một lần (SSO)
Diễn viên: Nhân viên.
Mô tả: Đăng nhập một lần để truy cập các hệ thống nội bộ.
Bước thực hiện:
Đăng nhập vào Identity Service.
Nhận token truy cập để sử dụng các hệ thống khác (giao dịch, báo cáo).


Kết quả: Truy cập liền mạch, giảm thao tác đăng nhập.



2.3. Giám sát và báo cáo

Use case 2.3.1: Ghi nhật ký hoạt động
Diễn viên: Hệ thống.
Mô tả: Ghi lại mọi hoạt động liên quan đến danh tính.
Bước thực hiện:
Ghi log các sự kiện: đăng nhập, đăng xuất, cập nhật thông tin, thay đổi vai trò.
Lưu trữ log an toàn, đảm bảo truy xuất khi cần.


Kết quả: Nhật ký được lưu trữ, sẵn sàng cho kiểm tra.


Use case 2.3.2: Tạo báo cáo hoạt động
Diễn viên: Quản trị viên.
Mô tả: Tạo báo cáo về hoạt động xác thực danh tính.
Bước thực hiện:
Chọn khoảng thời gian và loại báo cáo (đăng nhập, thay đổi vai trò).
Xuất báo cáo định dạng PDF hoặc CSV.


Kết quả: Báo cáo đáp ứng yêu cầu kiểm tra nội bộ hoặc cơ quan quản lý.



2.4. Tự động hóa và phân tích

Use case 2.4.1: Tự động vô hiệu hóa tài khoản
Diễn viên: Hệ thống.
Mô tả: Vô hiệu hóa tài khoản nhân viên nghỉ việc.
Bước thực hiện:
Nhận thông báo nghỉ việc từ hệ thống HRM.
Chuyển trạng thái tài khoản sang "Khóa".
Ghi log hành động vô hiệu hóa.


Kết quả: Tài khoản được vô hiệu hóa, giảm rủi ro bảo mật.


Use case 2.4.2: Phát hiện hành vi bất thường
Diễn viên: Hệ thống, quản trị viên.
Mô tả: Phân tích và cảnh báo hành vi đăng nhập bất thường.
Bước thực hiện:
Phân tích mẫu đăng nhập: thời gian, địa điểm, thiết bị.
Gửi cảnh báo thời gian thực nếu phát hiện bất thường (ví dụ: đăng nhập ngoài giờ làm việc).
Quản trị viên xem xét và xử lý.


Kết quả: Rủi ro bảo mật được phát hiện và xử lý kịp thời.



Kết luận
Identity Service cung cấp các use case nghiệp vụ tập trung vào quản lý danh tính nhân viên hệ thống chứng khoán, từ tạo tài khoản, xác thực, phân quyền đến giám sát và tự động hóa. Dịch vụ đảm bảo an toàn, hiệu quả và hỗ trợ vận hành giao dịch chứng khoán một cách minh bạch và chuyên nghiệp.
