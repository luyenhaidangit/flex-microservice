# Flex.Gateway.Yarp

API Gateway cho hệ thống microservices Flex sử dụng YARP (Yet Another Reverse Proxy) với hỗ trợ gRPC.

## Tổng quan

Flex.Gateway.Yarp là một API Gateway hiện đại được thiết kế để:

- **Routing**: Định tuyến request đến các microservice phù hợp
- **Load Balancing**: Cân bằng tải giữa các instance của service
- **Authentication & Authorization**: Xác thực và phân quyền tập trung
- **Observability**: Logging, metrics, và distributed tracing
- **Security**: Rate limiting, SSL termination, và các biện pháp bảo mật khác
- **Performance**: Tối ưu cho gRPC với HTTP/2

## Kiến trúc

```
Client → Flex.Gateway.Yarp → Microservices (gRPC)
                ↓
        Identity Service (JWT)
                ↓
        Observability Stack
```

### Các thành phần chính

1. **YARP Reverse Proxy**: Core routing và load balancing
2. **gRPC Support**: Hỗ trợ giao tiếp gRPC với HTTP/2
3. **JWT Authentication**: Tích hợp với Identity Service
4. **OpenTelemetry**: Observability và monitoring
5. **Rate Limiting**: Bảo vệ khỏi abuse
6. **Health Checks**: Kiểm tra sức khỏe service

## Cấu hình

### Routes

Gateway hỗ trợ routing cho các service sau:

- **System Service**: `/flex.system.grpc.services.BranchService/*`
- **Identity Service**: `/identity/*`
- **Securities Service**: `/securities/*`
- **Ordering Service**: `/ordering/*`
- **Inventory Service**: `/inventory/*`
- **Investor Service**: `/investor/*`
- **Basket Service**: `/basket/*`
- **Integration Service**: `/integration/*`
- **Job Service**: `/job/*`

### Clusters

Mỗi service được cấu hình thành cluster với:
- Load balancing policy (RoundRobin)
- Health checks (active và passive)
- HTTP/2 support
- Multiple destinations (nếu có)

## Triển khai

### Development

1. **Build project**:
```bash
dotnet build
```

2. **Run gateway**:
```bash
dotnet run
```

3. **Test gateway**:
```bash
python test_gateway.py
```

### Production với Docker

1. **Build Docker image**:
```bash
docker build -t flex-gateway-yarp .
```

2. **Run container**:
```bash
docker run -d \
  --name flex-gateway \
  -p 5000:5000 \
  -p 5001:5001 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  flex-gateway-yarp
```

### Kubernetes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: flex-gateway-yarp
spec:
  replicas: 2
  selector:
    matchLabels:
      app: flex-gateway-yarp
  template:
    metadata:
      labels:
        app: flex-gateway-yarp
    spec:
      containers:
      - name: gateway
        image: flex-gateway-yarp:latest
        ports:
        - containerPort: 5000
        - containerPort: 5001
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        livenessProbe:
          httpGet:
            path: /health
            port: 5000
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /health
            port: 5000
          initialDelaySeconds: 5
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: flex-gateway-service
spec:
  selector:
    app: flex-gateway-yarp
  ports:
  - name: http2
    port: 5000
    targetPort: 5000
  - name: https
    port: 5001
    targetPort: 5001
  type: LoadBalancer
```

## API Endpoints

### Health Check
- `GET /health` - Kiểm tra sức khỏe gateway

### Gateway Management
- `GET /api/gateway/status` - Trạng thái gateway
- `GET /api/gateway/routes` - Danh sách routes (Admin only)
- `GET /api/gateway/clusters` - Danh sách clusters (Admin only)
- `GET /api/gateway/metrics` - Metrics gateway

### Metrics
- Metrics được expose qua OpenTelemetry OTLP exporter

### gRPC Services
- Tất cả gRPC calls được proxy qua gateway theo cấu hình routes

## Monitoring

### Logs

Gateway sử dụng Serilog với các sink:
- Console (development)
- File (production)
- Structured logging với correlation ID

### Metrics

OpenTelemetry metrics được expose qua:
- OTLP exporter (nếu cấu hình)
- Runtime instrumentation

### Tracing

Distributed tracing được thực hiện qua:
- OpenTelemetry tracing
- Correlation ID propagation
- Span creation cho mỗi request

## Bảo mật

### Authentication

- JWT Bearer token validation
- Tích hợp với Identity Service
- Token forwarding đến backend services

### Authorization

- Policy-based authorization
- Role-based access control
- Admin-only endpoints

### Rate Limiting

- Fixed window rate limiting
- Configurable limits per endpoint
- Queue management cho burst traffic

### SSL/TLS

- HTTPS enforcement
- HTTP/2 support
- SSL termination tại gateway

## Performance

### Tối ưu hóa

- HTTP/2 multiplexing
- Connection pooling
- Async/await throughout
- Memory-efficient processing

### Load Balancing

- RoundRobin policy (mặc định)
- Health check integration
- Circuit breaker pattern
- Retry policies

### Scaling

- Horizontal scaling support
- Stateless design
- Kubernetes-ready
- Auto-scaling compatible

## Troubleshooting

### Common Issues

1. **gRPC connection failed**:
   - Kiểm tra backend service có chạy không
   - Verify HTTP/2 configuration
   - Check firewall rules

2. **Authentication failed**:
   - Verify JWT token
   - Check Identity Service connection
   - Validate audience/issuer

3. **High latency**:
   - Check backend service performance
   - Monitor connection pooling
   - Review rate limiting settings

### Debug Mode

Enable debug logging trong `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Yarp": "Debug",
      "Flex.Gateway.Yarp": "Debug"
    }
  }
}
```

## Development

### Adding New Service

1. **Update appsettings.json**:
```json
{
  "ReverseProxy": {
    "Routes": {
      "NewServiceRoute": {
        "ClusterId": "NewServiceCluster",
        "Match": {
          "Path": "/newservice/{**catch-all}"
        }
      }
    },
    "Clusters": {
      "NewServiceCluster": {
        "Destinations": {
          "NewServiceDestination": {
            "Address": "https://localhost:700X"
          }
        }
      }
    }
  }
}
```

2. **Test configuration**:
```bash
python test_gateway.py
```

### Custom Transforms

Implement `ITransformProvider` để thêm custom logic:

```csharp
public class CustomTransforms : ITransformProvider
{
    public void Apply(TransformBuilderContext context)
    {
        // Custom transform logic
    }
}
```

## Contributing

1. Fork repository
2. Create feature branch
3. Make changes
4. Add tests
5. Submit pull request

## License

Internal use only - Flex Microservices Platform
