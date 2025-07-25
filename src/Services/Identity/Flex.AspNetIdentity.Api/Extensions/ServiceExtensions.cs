﻿using Flex.EntityFrameworkCore.Oracle;
using Flex.AspNetIdentity.Api.Persistence;
using Microsoft.AspNetCore.Identity;
using Flex.AspNetIdentity.Api.Entities;
using Flex.Security;
using Flex.Shared.Constants;
using Flex.System.Api.Repositories;
using Flex.System.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.AspNetIdentity.Api.Services;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Repositories;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.Common;
using Flex.Shared.Extensions;

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
            // Bind JwtSettings settings
            var jwtSettings = configuration.GetRequiredSection<JwtSettings>(ConfigKeyConstants.JwtSettings);
            var systemUrlGrpc = configuration.GetRequiredValue<string>(ConfigKeyConstants.GrpcSettings_SystemUrl);

            // Add services to the container.
            services.AddControllers().ApplyJsonSettings();

            services.AddEndpointsApiExplorer();

            services.AddOpenApi();

            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            // Database
            services.ConfigureServiceDbContext<IdentityDbContext>(configuration, useWallet: true);

            // Identity
            services.ConfigureAspNetIdentity();
            services.AddAuthenticationJwtToken(configuration);

            // AutoMapper
            services.AddAutoMapper(AssemblyReference.Assembly);

            // Configure gRPC client
            //services.AddGrpcClient<System.Grpc.BranchService.BranchServiceClient>(options =>
            //{
            //    options.Address = new Uri(systemUrlGrpc);
            //}).ConfigureGrpcChannelOptions();

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRequestHeaderRepository, UserRequestHeaderRepository>();
            services.AddScoped<IUserRequestDataRepository, UserRequestDataRepository>();
            services.AddScoped<IUserAuditLogRepository, UserAuditLogRepository>();

            //services.AddScoped<IBranchService, BranchClientService>();

            return services;
        }

        //public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        //{
        //    services.AddScoped<IBranchService, BranchService>();
        //    services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        //    services.AddHttpClient("SystemApi", client =>
        //    {
        //        client.BaseAddress = new Uri("http://system-api/");
        //    });

        //    return services;
        //}

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Base
            services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>));
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

            // Services
            services.AddScoped<IRoleService, RoleService>();

            // Repositories
            services.AddScoped<IRoleRequestRepository, RoleRequestRepository>();

            return services;
        }
        //private static bool ConfigureGrpcChannelOptions(this IHttpClientBuilder builder)
        //{
        //    var methodConfigs = new MethodConfig
        //    {
        //        Names = { MethodName.Default },
        //        RetryPolicy = new RetryPolicy
        //        {
        //            MaxAttempts = 5,
        //            InitialBackoff = TimeSpan.FromSeconds(1),
        //            MaxBackoff = TimeSpan.FromSeconds(5),
        //            BackoffMultiplier = 1.5,
        //            RetryableStatusCodes =
        //            {
        //                // Whatever status codes we want to look for
        //                StatusCode.Unauthenticated, StatusCode.NotFound, StatusCode.Unavailable,
        //            }
        //        }
        //    };

        //    builder.ConfigureChannel(options =>
        //    {
        //        options.ServiceConfig = new ServiceConfig
        //        {
        //            MethodConfigs = { methodConfigs }
        //        };
        //    });

        //    return true;
        //}

        private static IServiceCollection ConfigureAspNetIdentity(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
        #endregion
    }
}
