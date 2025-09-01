using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Flex.AspNetIdentity.Api.Entities;
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
using Flex.AspNetIdentity.Api.Integrations.Interfaces;
using Flex.AspNetIdentity.Api.Integrations;
using Flex.AspNetIdentity.Api.Integrations.Configurations;

namespace Flex.AspNetIdentity.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtSettings>().Bind(configuration.GetSection(ConfigKeyConstants.JwtSettings)).ValidateDataAnnotations().ValidateOnStart();
            services.AddOptions<GrpcSettings>().Bind(configuration.GetSection(ConfigKeyConstants.GrpcSettings)).ValidateDataAnnotations().ValidateOnStart();

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

            // Grpc
            services.ConfigureGrpcClients(configuration);

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Base
            services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            // Repositories
            services.AddScoped<IRoleRequestRepository, RoleRequestRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRequestRepository, UserRequestRepository>();

            // Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Integrations
            services.AddScoped<IBranchIntegrationService, BranchGrpcService>();

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            return services;
        }

        private static IServiceCollection ConfigureGrpcClients(this IServiceCollection services, IConfiguration configuration)
        {
            ;
            var grpcSettings = configuration.GetRequiredSection<GrpcSettings>(ConfigKeyConstants.GrpcSettings);

            services.AddGrpcClient<BranchService.BranchServiceClient>(o =>
            {
                o.Address = new Uri(grpcSettings.SystemUrl);
            });

            return services;
        }
        #endregion
    }
}
