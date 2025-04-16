* Dịch Vụ SMS khách hàng
*- Bảng `SMSTYPE` là nơi lưu trữ thông tin cấu hình gói dịch vụ SMS, bao gồm số lượng SMS miễn phí (`SMSLIMIT`) và các thông số liên quan khác như phí phụ trội (`OVERFEE`) và VAT (`VAT`).
- Khi tính phí, hệ thống sẽ dựa vào bảng `SMSREGISLOG` hoặc bảng tương tự để lấy thông tin gói dịch vụ SMS mà khách hàng đã đăng ký.

<aside>
💡

Khách hàng đăng ký dịch vụ SMS qua giao dịch `0043` , hủy qua giao dịch `1198`
Batch cuối ngày quét, nếu khách hàng nợ phí quá số kỳ hạn cấu hình `MAXDEPT_SMSFEE` thì tự động call 1198 để hủy dịch vụ 

[Giao dịch 0043, 1198](https://www.notion.so/Giao-d-ch-0043-1198-1420e3afaa348121abb9f1022a147624?pvs=21)

</aside>

- Định kỳ, hệ thống sẽ kiểm tra và tính toán số lượng SMS mà khách hàng đã sử dụng, dựa trên các loại phí đã đăng ký tại batch `PR_CIFEESMSCAL`.  Bằng cách liên kết bảng `SMSREGISLOG` với bảng `SMSTYPE`, hệ thống sẽ tính được khách hàng đó có bao nhiêu SMS miễn phí `SMSLIMIT`. Hệ thống đếm số SMS mà khách hàng đã gửi trong kỳ tính phí (sử dụng các bảng log như `VW_EMAILLOG_ALL` trong ví dụ của bạn) `EmailLog`. Nếu số lượng SMS gửi đi vượt quá giới hạn miễn phí, hệ thống sẽ tính giá gói tin nhắn cộng thêm phí phụ trội (`l_FeeAmt = l_FeeAmt + GREATEST(l_TotalSend - rec.SMSLIMIT, 0) * rec.OVERFEE;`)
- Phí SMS được ghi nhận vào bảng `SMSFEESCHD`, trong đó bao gồm các thông tin như ID khách hàng, ngày bắt đầu, ngày kết thúc, số lượng SMS đã gửi, và tổng số tiền phí SMS khi một chu kỳ tính phí SMS hoàn tất vào ngày cấu hình thu phí mỗi tháng.
- Bảng `SMSFEESCHDHIST` được sử dụng để lưu trữ lịch sử phí SMS sau khi khách hàng đã thanh toán phí. Khi một bản ghi trong `SMSFEESCHD` đã được thanh toán đầy đủ (phí và VAT), nó sẽ được di chuyển sang `SMSFEESCHDHIST` để lưu trữ lịch sử và xóa khỏi bảng `SMSFEESCHD` bước bước batch quét hàng ngày
- Tại bước batch hàng ngày nếu là ngày thu phí được cấu hình trong gói, hệ thống quét bảng `SMSFEESCHD` để tìm các khoản phí SMS chưa thanh toán đầy đủ. Với mỗi khách hàng, hệ thống kiểm tra các tiểu khoản để tìm tài khoản phù hợp có đủ số dư khả dụng (`BALDEFOVD`) để thanh toán phí, ưu tiên tiểu khoản thường (mrtype = 'N'), rồi đến tài khoản ký quỹ. Nếu tìm được tiểu khoản phù hợp của khách hàng thực hiện (`Giao dịch 1197`) để thực hiện thu phí.
- Chấp nhận việc không thu được phí SMS khi tài khoản khách hàng không đủ. Tự động cắt dịch vụ SMS nếu khách hàng quá hạn 2 kỳ
- Khi muốn hủy dịch vụ SMS giao dịch `1198` không phải do bước batch gọi mà thực hiện bằng tay, thực hiện ghi nhận một bản ghi trong `SMSFEESCHD` với số tin nhắn từ lúc đăng ký đến ngày hủy, sau đó lấy các tiểu khoản và chọn ra tiểu khoản đủ tiền để thực hiện giao dịch 1197 trả phí SMS