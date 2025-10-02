using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.EntityFrameworkCore.Oracle;
using Flex.Infrastructure.Json;
using Flex.Infrastructure.Redis;
using Flex.Infrastructure.Swashbuckle;
using Flex.Notification.Api.Persistence;
using Flex.Notification.Api.Repositories;
using Flex.Notification.Api.Repositories.Interfaces;
using Flex.Notification.Api.Services;
using Flex.Notification.Api.Services.Interfaces;
using Flex.Shared.Extensions;
using MassTransit;

namespace Flex.Notification.Api.Extensions
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
            services.ConfigureServiceDbContext<NotificationDbContext>(configuration, useWallet: true);
            services.ConfigureStackExchangeRedisCache(configuration);

            // Grpc
            services.AddGrpc();

            // RabbitMQ
            services.ConfigureMassTransit(configuration);

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();

            // Services
            services.AddScoped<INotificationTemplateService, NotificationTemplateService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }

        private static IServiceCollection ConfigureMassTransit(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("213.35.100.75", 5672, "/", h =>
                    {
                        h.Username("admin");
                        h.Password("Admin#12345");
                    });
                });
            });

            return services;
        }
        #endregion
    }
}
