using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Flex.Gateway.Yarp.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var reverseProxySettings = configuration.GetSection("ReverseProxy");

            // Add YARP Reverse Proxy
            services.AddReverseProxy()
                    .LoadFromConfig(reverseProxySettings)
                    .AddTransforms<GatewayTransforms>();

            // Add gRPC support
            services.AddGrpc();

            // Add Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                options.Authority = jwtSettings["Authority"];
                options.Audience = jwtSettings["Audience"];
                options.RequireHttpsMetadata = bool.Parse(jwtSettings["RequireHttpsMetadata"] ?? "true");
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Log.Warning("Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Log.Information("Token validated for user: {User}", context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    }
                };
            });

            // Add Authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAuthenticatedUser", policy =>
                    policy.RequireAuthenticatedUser());
                
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));
            });

            // Add Rate Limiting - Sử dụng middleware đơn giản thay vì package
            services.AddRateLimiter(options =>
            {
                var rateLimitSettings = configuration.GetSection("RateLimiting");
                var permitLimit = int.Parse(rateLimitSettings["DefaultPermitLimit"] ?? "100");
                var window = TimeSpan.Parse(rateLimitSettings["DefaultWindow"] ?? "00:01:00");
                var queueLimit = int.Parse(rateLimitSettings["DefaultQueueLimit"] ?? "10");

                options.AddFixedWindowLimiter("Default", limiterOptions =>
                {
                    limiterOptions.PermitLimit = permitLimit;
                    limiterOptions.Window = window;
                    limiterOptions.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    limiterOptions.QueueLimit = queueLimit;
                });
            });

            // Add Health Checks
            services.AddHealthChecks()
                .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

            // Add OpenTelemetry
            services.AddOpenTelemetry()
                .WithTracing(builder =>
                {
                    builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddSource("Flex.Gateway.Yarp")
                        .SetResourceBuilder(OpenTelemetry.Resources.ResourceBuilder.CreateDefault()
                            .AddService(serviceName: "Flex.Gateway.Yarp", serviceVersion: "1.0.0"))
                        .AddOtlpExporter(opts => opts.Endpoint = new Uri(configuration["OpenTelemetry:OtlpEndpoint"] ?? "http://localhost:4317"));
                })
                .WithMetrics(builder =>
                {
                    builder
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                        //.AddRuntimeInstrumentation();
                });

            // Add Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/gateway-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            return services;
        }
    }

    public class GatewayTransforms : ITransformProvider
    {
        public void ValidateRoute(TransformRouteValidationContext context)
        {
        }

        public void ValidateCluster(TransformClusterValidationContext context)
        {
        }

        public void Apply(TransformBuilderContext context)
        {
            // Add correlation ID header
            context.AddRequestTransform(async context =>
            {
                var correlationId = context.HttpContext.TraceIdentifier;
                context.ProxyRequest.Headers.Add("X-Correlation-ID", correlationId);
                context.ProxyRequest.Headers.Add("X-Gateway", "YARP");
                
                // Forward user information if authenticated
                if (context.HttpContext.User.Identity?.IsAuthenticated == true)
                {
                    var userId = context.HttpContext.User.FindFirst("sub")?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        context.ProxyRequest.Headers.Add("X-User-ID", userId);
                    }
                }
            });

            // Add response headers
            context.AddResponseTransform(async context =>
            {
                context.HttpContext.Response.Headers.Add("X-Gateway-Version", "1.0.0");
                context.HttpContext.Response.Headers.Add("X-Processed-By", "Flex.Gateway.Yarp");
            });
        }
    }
}
