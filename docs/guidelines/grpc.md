Dưới đây là bộ “best practices” gRPC cho .NET (server & client) kèm ví dụ code áp dụng ngay. Mình chia theo 10 nhóm: thiết kế API, hợp đồng `.proto`, server, client, lỗi & retry, bảo mật, hiệu năng, quan sát (observability), test, và triển khai (K8s/AWS).

# 1) Thiết kế API (tư duy tổng thể)

* Ưu tiên **unary** cho tác vụ ngắn; dùng **server streaming** để đẩy dữ liệu theo lô; **client/bidi streaming** cho upload/log real-time.
* Tư duy **nhiệm vụ hẹp** (micro RPC). Tránh “God RPC” đa mục tiêu.
* **Idempotency** cho các lệnh có thể gọi lại (đính kèm `request_id`).
* Chuẩn hóa **status** & **error model** ngay từ đầu (mục #5).
* Luôn đặt **deadline** ở client, **cancellation** ở server.
* Lập kế hoạch **versioning** (mục #2).

# 2) Hợp đồng `.proto` (schema-first)

**Quy tắc:**

* Field mới chỉ **thêm** với tag số mới; **không đổi số** hay xóa field đang dùng.
* Dùng `oneof` thay vì field rỗng tương hỗ.
* Tên gói/namespace có **version**: `package my.company.orders.v1;`
* Không lạm dụng `google.protobuf.Any`.

**Ví dụ `orders.proto`:**

```proto
syntax = "proto3";

package my.company.orders.v1;
option csharp_namespace = "My.Company.Orders.V1";

service OrderService {
  rpc CreateOrder (CreateOrderRequest) returns (CreateOrderReply);
  rpc GetOrder    (GetOrderRequest)   returns (GetOrderReply);
  rpc StreamOrders(StreamOrdersRequest) returns (stream OrderSummary);
}

message Money {
  string currency = 1; // ISO 4217
  int64  units    = 2; // smallest unit (cents)
}

message OrderLine {
  string sku   = 1;
  int32  qty   = 2;
  Money  price = 3;
}

message CreateOrderRequest {
  string request_id = 1; // idempotency key
  string customer_id = 2;
  repeated OrderLine lines = 3;
  map<string,string> metadata = 4;
}

message CreateOrderReply { string order_id = 1; }

message GetOrderRequest { string order_id = 1; }
message GetOrderReply   { string order_id = 1; repeated OrderLine lines = 2; Money total = 3; }

message StreamOrdersRequest { string customer_id = 1; int32 page_size = 2; }
message OrderSummary { string order_id = 1; Money total = 2; int64 created_unix = 3; }
```

# 3) Server ASP.NET Core gRPC (Kestrel)

**Program.cs (NET 8):**

```csharp
var builder = WebApplication.CreateBuilder(args);

// Kestrel HTTP/2 (và HTTP/3 nếu cần)
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(5005, lo => lo.Protocols = HttpProtocols.Http2); // gRPC
    // o.ListenAnyIP(5006, lo => lo.Protocols = HttpProtocols.Http3); // tùy chọn
});

// DI, HealthChecks, gRPC + gRPC-Reflection (dev only)
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.MaxReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    options.MaxSendMessageSize = 10 * 1024 * 1024;
    // Interceptors: logging/validation/tracing (mục #8)
});
builder.Services.AddGrpcReflection();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
}
app.MapGrpcService<OrderServiceImpl>();
app.MapHealthChecks("/health"); // dùng cho K8s
app.MapGet("/", () => "gRPC server is running.");

app.Run();
```

**Service implementation (deadline/cancel, logging, idempotency):**

```csharp
public class OrderServiceImpl : OrderService.OrderServiceBase
{
    private readonly ILogger<OrderServiceImpl> _logger;

    public OrderServiceImpl(ILogger<OrderServiceImpl> logger) => _logger = logger;

    public override async Task<CreateOrderReply> CreateOrder(CreateOrderRequest req, ServerCallContext ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        // Enforce deadline-aware calls
        if (ctx.Deadline != DateTime.MaxValue && ctx.Deadline < DateTime.UtcNow)
            throw new RpcException(new Status(StatusCode.DeadlineExceeded, "Deadline exceeded"));

        // Idempotency check (ví dụ)
        // if (await _repo.HasProcessed(req.RequestId)) return existingResult;

        // Validate đơn giản
        if (string.IsNullOrWhiteSpace(req.CustomerId) || req.Lines.Count == 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Missing required fields"));

        // Xử lý...
        var orderId = Guid.NewGuid().ToString("N");

        _logger.LogInformation("Created order {OrderId} for {CustomerId}", orderId, req.CustomerId);

        return new CreateOrderReply { OrderId = orderId };
    }

    public override async Task StreamOrders(StreamOrdersRequest req, IServerStreamWriter<OrderSummary> responseStream, ServerCallContext ctx)
    {
        // stream chunked
        foreach (var item in await GetSummariesAsync(req.CustomerId, req.PageSize, ctx.CancellationToken))
        {
            await responseStream.WriteAsync(item);
        }
    }
}
```

**gRPC-Web (nếu cần trình duyệt):**

* Thêm package `Grpc.AspNetCore.Web`, bật:

```csharp
builder.Services.AddGrpc();
var app = builder.Build();
app.UseGrpcWeb(); // trước MapGrpcService
app.MapGrpcService<OrderServiceImpl>().EnableGrpcWeb();
```

* Dùng proxy (Envoy/Ingress) khi cần.

# 4) Client .NET (typed client + Channel reuse)

**Đăng ký typed clients với HttpClientFactory:**

```csharp
builder.Services.AddGrpcClient<OrderService.OrderServiceClient>("orders", o =>
{
    o.Address = new Uri("https://orders.mycorp.local:5005");
})
// Retry + timeout qua ServiceConfig (mục #5) hoặc PollyHandler nếu cần
.ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
    EnableMultipleHttp2Connections = false // giữ ít connection, tái sử dụng
});
```

**Gọi với deadline/cancel, metadata:**

```csharp
var client = sp.GetRequiredService<GrpcClientFactory>().CreateClient<OrderService.OrderServiceClient>("orders");

var headers = new Metadata
{
    { "x-request-id", Guid.NewGuid().ToString("N") },
    { "x-tenant", "tcbs" }
};

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
var reply = await client.CreateOrderAsync(
    new CreateOrderRequest { RequestId = "...", CustomerId = "C001" },
    headers,
    deadline: DateTime.UtcNow.AddSeconds(2),
    cancellationToken: cts.Token
);
```

# 5) Lỗi, Retry, và Status

**Nguyên tắc:**

* Server ném `RpcException` với `StatusCode` phù hợp: `InvalidArgument`, `NotFound`, `AlreadyExists`, `PermissionDenied`, `Unauthenticated`, `ResourceExhausted`, `Unavailable`, `Internal`, `DeadlineExceeded`…
* **Đính kèm chi tiết** qua **trailer metadata** (không lộ PII).
* **Client retry** chỉ cho lỗi **tạm thời**: `Unavailable`, `DeadlineExceeded`, `ResourceExhausted`. Không retry lỗi nghiệp vụ.

**ServiceConfig cho retry (client-side):**

```csharp
var serviceConfig = new ServiceConfig
{
    MethodConfigs =
    {
        new MethodConfig
        {
            Names = { MethodName.Default }, // áp dụng toàn bộ
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 4,
                InitialBackoff = TimeSpan.FromMilliseconds(200),
                MaxBackoff = TimeSpan.FromSeconds(3),
                BackoffMultiplier = 2.0,
                RetryableStatusCodes = { StatusCode.Unavailable, StatusCode.DeadlineExceeded, StatusCode.ResourceExhausted }
            },
            Timeout = TimeSpan.FromSeconds(3)
        }
    }
};

var channel = GrpcChannel.ForAddress("https://orders.mycorp.local:5005", new GrpcChannelOptions
{
    ServiceConfig = serviceConfig,
    HttpHandler = new SocketsHttpHandler()
});
var client = new OrderService.OrderServiceClient(channel);
```

# 6) Bảo mật (TLS, mTLS, JWT)

* **TLS** bắt buộc trên internet. Trong nội bộ, vẫn nên TLS (zero-trust).
* **mTLS** giữa service-to-service (EKS + cert rotation).
* **AuthN**: Bearer JWT (OIDC) hoặc mTLS.
  **AuthZ**: kiểm tra scope/role theo RPC (interceptor).
* **Metadata** không chứa PII nhạy cảm; nếu cần mã hóa phía client.

**Thêm JWT ở server:**

```csharp
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", o =>
    {
        o.Authority = "https://identity.mycorp.local";
        o.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization();
app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<OrderServiceImpl>();
```

**Client kèm token:**

```csharp
var creds = CallCredentials.FromInterceptor(async (ctx, meta) =>
{
    var token = await tokenProvider.GetAccessTokenAsync();
    meta.Add("Authorization", $"Bearer {token}");
});
var channel = GrpcChannel.ForAddress("https://...", new GrpcChannelOptions
{
    Credentials = ChannelCredentials.Create(new SslCredentials(), creds)
});
```

# 7) Hiệu năng & khả năng mở rộng

* **Tái sử dụng channel** & client (singleton hoặc scoped cao) — tạo mới liên tục sẽ tốn kém.
* Bật **HTTP/2 keepalive** (đã minh họa ở client).
* Chọn **streaming** thay vì trả danh sách rất lớn; **paging** nếu dùng unary.
* Cẩn trọng **MaxMessageSize**; cân nhắc **compression** (gzip) cho payload lớn.
* Serializer **Google.Protobuf** rất nhanh — tránh mapping DTO rườm rà.
* “**Async all the way**”; không chặn `.Result`/`.Wait()`.
* **Batched RPC** hoặc gom nhiều item trong một stream khi phù hợp.
* Giới hạn **concurrency** trên server (Channel, I/O, DB) + **circuit breaker** tại client.

# 8) Logging, Metrics, Tracing (OpenTelemetry)

* Dùng **OpenTelemetry**: `AddOpenTelemetry().WithTracing().WithMetrics()`.
* Xuất trace/metric sang OTLP Collector (Prometheus/Grafana/Tempo/Jaeger).
* Log **structured** (Serilog) + **correlation-id** từ metadata.

**Interceptor logging (server):**

```csharp
public class LoggingInterceptor : Interceptor
{
    private readonly ILogger<LoggingInterceptor> _logger;
    public LoggingInterceptor(ILogger<LoggingInterceptor> logger) => _logger = logger;

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var rid = context.RequestHeaders.GetValue("x-request-id") ?? Guid.NewGuid().ToString("N");
        using (_logger.BeginScope(new Dictionary<string, object> { ["request_id"] = rid }))
        {
            _logger.LogInformation("RPC {Method} start", context.Method);
            try
            {
                var resp = await continuation(request, context);
                _logger.LogInformation("RPC {Method} OK", context.Method);
                return resp;
            }
            catch (RpcException ex)
            {
                _logger.LogWarning(ex, "RPC {Method} failed: {Status}", context.Method, ex.StatusCode);
                throw;
            }
        }
    }
}
```

> Đăng ký: `builder.Services.AddGrpc(o => o.Interceptors.Add<LoggingInterceptor>());`

# 9) Testing (Unit/Integration/Contract)

* **Contract-first**: lock file `.proto` (golden file).
* **Unit test** service logic thuần C# (không cần wire gRPC).
* **Integration test** với `GrpcChannel.ForAddress` + `GrpcTestFixture` (Kestrel/`WebApplicationFactory`).
* **Golden tests**: phản hồi mẫu để bắt regressions.
* **Load test**: k6/Vegeta/Gatling (qua gRPC), hoặc ghz.

**Integration test mẫu (xUnit):**

```csharp
public class GrpcFixture : IAsyncLifetime
{
    public WebApplicationFactory<Program> Factory { get; private set; } = default!;
    public OrderService.OrderServiceClient Client { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        Factory = new WebApplicationFactory<Program>();
        var httpClient = Factory.CreateDefaultClient(new ResponseVersionHandler()); // HTTP/2 handler
        var channel = GrpcChannel.ForAddress(httpClient.BaseAddress!, new GrpcChannelOptions { HttpClient = httpClient });
        Client = new OrderService.OrderServiceClient(channel);
        await Task.CompletedTask;
    }
    public Task DisposeAsync() => Task.CompletedTask;
}

public class OrderTests(GrpcFixture fx) : IClassFixture<GrpcFixture>
{
    [Fact]
    public async Task CreateOrder_Should_Return_Id()
    {
        var res = await fx.Client.CreateOrderAsync(new CreateOrderRequest { CustomerId = "C001" });
        Assert.False(string.IsNullOrEmpty(res.OrderId));
    }
}
```

# 10) Triển khai (Kubernetes/EKS/Ingress)

* **Health probes**: `/health` (HTTP) hoặc `grpc_health_v1` (native health).
* **HPA** theo CPU/RAM & RPS; đặt **requests/limits** rõ ràng.
* **Ingress/LoadBalancer** hỗ trợ HTTP/2:

  * AWS ALB/NLB hỗ trợ gRPC; đảm bảo **HTTP/2** tới target.
  * Nếu dùng **gRPC-Web**, cần cấu hình proxy (Envoy/Ingress-nginx).
* **TLS** end-to-end. Tự động gia hạn cert (ACM/Cert-Manager).
* **Config** qua `appsettings` + biến môi trường; không hard-code endpoint.
* **Canary/Blue-Green** khi nâng version RPC. Giữ song song v1 & v2.

---

## Checklist thực thi nhanh

1. Định nghĩa `.proto` có version + quy ước số tag.
2. Bật deadline bắt buộc từ client; server bắt `CancellationToken`.
3. Chuẩn hóa `StatusCode`, **không** dùng `Internal` cho lỗi nghiệp vụ.
4. Client: **reuse channel/client**, bật retry policy cho lỗi tạm thời.
5. Bật TLS/mTLS; JWT + Authorization theo RPC.
6. Streaming khi dữ liệu lớn/real-time.
7. Quan sát: OpenTelemetry (trace/metrics/log).
8. Health checks & readiness; autoscale.
9. Test: unit + integration + golden + load.
10. Kế hoạch versioning & backward compatibility.

Nếu bạn muốn, mình có thể:

* Sinh **template solution** .NET 8 (Server + Client + Tests + Dockerfile + Helm chart) dựa trên các best practice trên.
* Chuyển **API hiện tại** của bạn sang gRPC từng bước (mapping REST ↔ gRPC).
* Thêm **interceptor** sẵn: logging, validation (FluentValidation), authz theo scope.
