using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.Common;
using Flex.System.Api.Persistence;
using Flex.System.Api.Repositories.Interfaces;
using Flex.System.Api.Repositories;
using Flex.Infrastructure.Swashbuckle;
using Flex.Shared.Extensions;
using Flex.Infrastructure.EntityFrameworkCore.Oracle;
using Flex.System.Api.Services.Interfaces;
using Flex.System.Api.Services;
using Flex.Infrastructure.Redis;
using Flex.System.Api.Grpc;
using Flex.Infrastructure.Json;
using MassTransit;

namespace Flex.System.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(AssemblyReference.Assembly);
            services.ConfigureJsonOptionsDefault();
            services.ConfigureSwagger();
            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            // Database
            services.ConfigureServiceDbContext<SystemDbContext>(configuration, useWallet: true);
            services.ConfigureStackExchangeRedisCache(configuration);

            // Grpc
            services.AddGrpc();

            // RabbitMQ
            services.ConfigureRabbitMQ(configuration);

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                           .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                           .AddScoped<IConfigRepository, ConfigRepository>()
                           .AddScoped<IBranchRepository, BranchRepository>()
                           .AddScoped<IBranchRequestRepository, BranchRequestRepository>()
                           .AddScoped<IBranchService, BranchService>()
                           .AddScoped<IBranchEventPublisher, BranchEventPublisher>()
                           .AddScoped<IUserService, UserService>()
                           .AddScoped<BranchGrpcService>();
        }

        private static IServiceCollection ConfigureRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMQConfig = configuration.GetSection("RabbitMQ");
            
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMQConfig["Host"], rabbitMQConfig["Port"], rabbitMQConfig["VirtualHost"], h =>
                    {
                        h.Username(rabbitMQConfig["Username"]);
                        h.Password(rabbitMQConfig["Password"]);
                    });

                    // Configure retry policy
                    cfg.UseMessageRetry(r => r.Interval(
                        rabbitMQConfig.GetValue<int>("RetryCount"), 
                        TimeSpan.Parse(rabbitMQConfig["RetryDelay"] ?? "00:00:05")));

                    // Configure error handling
                    cfg.UseInMemoryOutbox();
                });
            });

            return services;
        }
        #endregion
    }
}
