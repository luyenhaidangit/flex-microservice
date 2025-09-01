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
            services.ConfigureJsonOptionsDefault();
            services.ConfigureSwagger();
            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            // Database
            services.ConfigureServiceDbContext<SystemDbContext>(configuration, useWallet: true);
            services.ConfigureStackExchangeRedisCache(configuration);

            // AutoMapper
            services.AddAutoMapper(AssemblyReference.Assembly);

            // Grpc
            services.AddGrpc();

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
                           .AddScoped<IUserService, UserService>()
                           .AddScoped<BranchGrpcService>();
        }
        #endregion
    }
}
