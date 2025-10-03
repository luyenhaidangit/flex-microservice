using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.EntityFrameworkCore.Oracle;
using Flex.Infrastructure.Json;
using Flex.Infrastructure.Workflow.Persistence;
using Flex.Shared.Extensions;
using Flex.Security;
using Microsoft.Extensions.Configuration;

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
            services.ConfigureJsonOptionsDefault();
            services.AddOpenApi();
            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            services.AddInfrastructureServices();

            // Database (Oracle via wallet consistent with other services)
            services.ConfigureServiceDbContext<WorkflowDbContext>(configuration, useWallet: true);

            // Auth
            services.AddAuthenticationJwtToken(configuration);

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            // Repositories
            services.AddScoped<Repositories.Interfaces.IWorkflowRequestRepository, Repositories.WorkflowRequestRepository>();
            services.AddScoped<Repositories.Interfaces.IWorkflowStepRepository, Repositories.WorkflowStepRepository>();

            // Services
            services.AddScoped<Services.Interfaces.ICurrentUserService, Services.CurrentUserService>();
            services.AddScoped<Services.Interfaces.IWorkflowRequestService, Services.WorkflowRequestService>();

            return services;
        }
    }
}

