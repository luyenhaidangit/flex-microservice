* Project:
- Flex.OcelotApiGateway:
	+ Routing: Định tuyến các request đến các endpoint của các service.
	+ Aggregation: Hợp nhất dữ liệu từ nhiều service thành một response duy nhất.
	+ Load balancing: Hỗ trợ cân bằng tải giữa các instance của service.
	+ Authentication & Authorization: Hỗ trợ xác thực và phân quyền, giúp bảo mật các dịch vụ bên dưới.
	+ Caching và Rate Limiting: Giúp cải thiện hiệu năng và kiểm soát số lượng request.
	+ Resiliency và Error Handling:
	+ Logging & Monitoring: Timeout và circuit breaker khi gọi các service phía dưới (có thể tích hợp với Polly), Cung cấp fallback response