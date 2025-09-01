using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Interceptors;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.EntityFrameworkCore.Oracle;
using Flex.Infrastructure.Json;
using Flex.Infrastructure.Redis;
using Flex.Security;
using Flex.Shared.Authorization;
using Flex.Shared.Constants;
using Flex.Shared.Extensions;
using Flex.System.Grpc.Services;
using Grpc.Core;
using Grpc.Net.Client.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Flex.AspNetIdentity.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtSettings>().Bind(configuration.GetSection(ConfigKeyConstants.JwtSettings)).ValidateDataAnnotations().ValidateOnStart();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers();
            services.ConfigureJsonOptionsDefault();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();
            services.AddOpenApi();
            services.AddAutoMapper(AssemblyReference.Assembly);
            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            // Database
            services.ConfigureServiceDbContext<IdentityDbContext>(configuration, useWallet: true);
            services.ConfigureStackExchangeRedisCache(configuration);

            // Auth
            services.AddAuthenticationJwtToken(configuration);

            // Configure gRPC clients
            services.ConfigureGrpcClients(configuration);

            // OpenTelemetry
            services.AddOpenTelemetry()
                .WithTracing(t => t.AddGrpcClientInstrumentation());
                //.WithMetrics(m => m.AddGrpcClientInstrumentation());

            return services;
        }

        private static IServiceCollection ConfigureGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            var cfg = configuration;

            // Common SocketsHttpHandler cho HTTP/2
            SocketsHttpHandler CreateHandler()
            {
                var g = cfg.GetSection("Grpc:Http2");
                return new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = TimeSpan.FromSeconds(g.GetValue<int>("PooledConnectionIdleTimeoutSeconds", 120)),
                    KeepAlivePingDelay = TimeSpan.FromSeconds(g.GetValue<int>("KeepAlivePingDelaySeconds", 20)),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(g.GetValue<int>("KeepAlivePingTimeoutSeconds", 10)),
                    EnableMultipleHttp2Connections = g.GetValue("EnableMultipleHttp2Connections", true)
                };
            }

            // ServiceConfig với Retry/Hedging (cho SystemService)
            var sysRetry = cfg.GetSection("Grpc:SystemService:Retry");
            var serviceConfig = new ServiceConfig
            {
                MethodConfigs =
                {
                    new MethodConfig
                    {
                        Names = { MethodName.Default },
                        RetryPolicy = new RetryPolicy
                        {
                            MaxAttempts = sysRetry.GetValue<int?>("MaxAttempts") ?? 4,
                            InitialBackoff = TimeSpan.Parse(sysRetry.GetValue<string>("InitialBackoff") ?? "00:00:00.2"),
                            MaxBackoff = TimeSpan.Parse(sysRetry.GetValue<string>("MaxBackoff") ?? "00:00:02"),
                            BackoffMultiplier = sysRetry.GetValue<double?>("BackoffMultiplier") ?? 2.0,
                            RetryableStatusCodes = {
                                Grpc.Core.StatusCode.Unavailable,
                                Grpc.Core.StatusCode.DeadlineExceeded
                            }
                        }
                    }
                }
            };

            // Interceptors
            //services.AddSingleton<ClientLoggingInterceptor>();
            services.AddSingleton<AuthHeaderInterceptor>();
            services.AddSingleton<CorrelationIdInterceptor>();

            // SystemService client
            services
                .AddGrpcClient<BranchService.BranchServiceClient>((sp, o) =>
                {
                    o.Address = new Uri(cfg["Grpc:SystemService:Address"]!);
                    o.ChannelOptionsActions.Add(ch =>
                    {
                        ch.Credentials = ChannelCredentials.SecureSsl; // TLS
                        ch.ServiceConfig = serviceConfig;              // retry/hedging
                    });
                    //o.HttpHandler = CreateHandler();
                })
                .AddInterceptor<AuthHeaderInterceptor>()
                .AddInterceptor<CorrelationIdInterceptor>();
                //.AddInterceptor<ClientLoggingInterceptor>();

            // Health check client
            //services.AddGrpcClient<Health.HealthClient>((sp, o) =>
            //{
            //    o.Address = new Uri(cfg["Grpc:SystemService:Address"]!);
            //    o.HttpHandler = CreateHandler();
            //    o.ChannelOptionsActions.Add(ch => ch.Credentials = ChannelCredentials.SecureSsl);
            //})
            //.AddInterceptor<AuthHeaderInterceptor>()
            //.AddInterceptor<CorrelationIdInterceptor>()
            //.AddInterceptor<ClientLoggingInterceptor>();

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Base
            services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            // Services
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Repositories
            services.AddScoped<IRoleRequestRepository, RoleRequestRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRequestRepository, UserRequestRepository>();

            // gRPC Gateway Services
            services.AddScoped<ISystemGateway, SystemGateway>();
            //services.AddScoped<GrpcHealthProbe>();

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            return services;
        }
        #endregion
    }
}
