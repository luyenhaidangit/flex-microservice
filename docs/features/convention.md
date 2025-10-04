Backend:
- Không sử dụng Data Annotation mapping DB khi khai báo entity mà sử dụng Fluent API trong IEntityTypeConfiguration<T>
- Tên bảng database đặt dựa theo tên class trong source Backend theo quy tắc in hoa, dùng số nhiều, tên class là PascalCase thì bảng sẽ dùng snake case với dấu gạch dưới giữa các từ. (VD: WorkflowDefinition → WORKFLOW_DEFINITIONS)
- Tại Configuration cần khai rõ kiểu dữ liệu database
- Khi tạo một file mới không thực hiện thêm mới dư 1 dòng cuối file khi tạo.
-----------------------------------------------
Database:
- Khi tạo bảng cần đặt tên constraint rõ ràng, tránh để hệ thống tự sinh tên vì dễ lệch giữa các môi trường (UAT, Prod) và gây khó khăn khi cần drop.
- Phân tích xem trường nào cần là kiểu date để tránh ToDate() nhiều lần và hơn hết là khi ToDate() so sánh sẽ mất index.
- Xem xét sử dụng Guid cho Id khi thiết kế microservice (tránh trùng id), có chức năng set key tại client hoặc trước khi insert, phân tán, đa vùng, cần expose ra ngoài API mà không muốn lộ số thứ tự,... 
- Thiết kế database trường Id kiểu Guid nếu ít record, vài trăm đến vài nghìn template thì dùng Guid thoải mái, không lo hiệu năng, nhưng hiện tại thì ưu tiên kiểu số tự tăng cho tôi.
- Thiết kế bảng, cá nhân tôi không thích tạo ràng buộc.
- Các bảng có trường ID để tự động tăng.
- Thiết kế db khi có những trường như ENTITY_KEY ref tới key của một bảng khác mà không biết kiểu dữ liệu của key bảng kia thì nên đặt là NVARCHAR.
- Khi thiết kế tên cột cần lưu ý tránh trùng với từ khóa hệ thống, ví dụ COMMENT.