using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.EntityFrameworkCore.Oracle;
using Flex.Infrastructure.Json;
using Flex.Infrastructure.Redis;
using Flex.Infrastructure.Swashbuckle;
using Flex.Shared.Extensions;
using Flex.Workflow.Api.Persistence;
using Flex.Workflow.Api.Repositories;
using Flex.Workflow.Api.Repositories.Interfaces;
using Flex.Workflow.Api.Services;
using Flex.Workflow.Api.Services.Interfaces;
using Flex.Workflow.Api.Services.Policy;
using Flex.Workflow.Api.Services.Steps;
using MassTransit;

namespace Flex.Workflow.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();
            services.AddAutoMapper(AssemblyReference.Assembly);
            services.ConfigureJsonOptionsDefault();
            services.ConfigureSwagger();
            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Database (Oracle)
            services.ConfigureServiceDbContext<WorkflowDbContext>(configuration, useWallet: true);

            // Redis Cache (optional)
            services.ConfigureStackExchangeRedisCache(configuration);

            // RabbitMQ (MassTransit)
            services.ConfigureRabbitMQ(configuration);

            // DI
            services.AddInfrastructureServices();

            // Hosted services
            services.AddHostedService<OutboxDispatcherHostedService>();

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services
                .AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                .AddScoped<IWorkflowDefinitionRepository, WorkflowDefinitionRepository>()
                .AddScoped<IWorkflowRequestRepository, WorkflowRequestRepository>()
                .AddScoped<IWorkflowActionRepository, WorkflowActionRepository>()
                .AddScoped<IWorkflowAuditLogRepository, WorkflowAuditLogRepository>()
                .AddScoped<IWorkflowOutboxRepository, WorkflowOutboxRepository>()
                .AddScoped<IWorkflowDefinitionService, WorkflowDefinitionService>()
                .AddScoped<IRequestService, RequestService>()
                .AddSingleton<IPolicyEvaluator, SimplePolicyEvaluator>()
                .AddSingleton<IStepResolver, SimpleStepResolver>();
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

                    cfg.UseMessageRetry(r => r.Interval(
                        rabbitMQConfig.GetValue<int>("RetryCount"),
                        TimeSpan.Parse(rabbitMQConfig["RetryDelay"] ?? "00:00:05")));

                    cfg.UseInMemoryOutbox();
                });
            });

            return services;
        }
    }
}
