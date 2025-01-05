using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.Common;
using Flex.Infrastructure.Swashbuckle;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Flex.Shared.SeedWork;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;
using Flex.Ordering.Infrastructure.Persistence;
using Flex.Ordering.Application.Common.Interfaces;
using Flex.Ordering.Infrastructure.Repositories;
using MassTransit;

namespace Flex.Ordering.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers(options =>
            {
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.WriteIndented = true;
            });

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.ConfigureSwagger();

            // Database
            services.ConfigureOrderingDbContext(configuration);

            // AutoMapper
            services.ConfigureAutoMapper();

            // Infrastructure
            services.AddInfrastructureServices();

            // Response
            services.ConfigureValidationErrorResponse();

            // Cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("https://localhost:7179") 
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.AddMediatR(cfg =>cfg.RegisterServicesFromAssembly(Flex.Ordering.Application.AssemblyReference.Assembly));

            return services;
        }

        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        #region Infrastructure
        private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<LowerCaseDocumentFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        private static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Flex.Ordering.Application.AssemblyReference.Assembly);

            return services;
        }

        private static IServiceCollection ConfigureOrderingDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;

            services.AddDbContext<OrderingDbContext>(options =>
            options.UseOracle(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                           .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                           .AddScoped<IOrderRepository, OrderRepository>();
        }

        private static IServiceCollection ConfigureValidationErrorResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                        );

                    var response = new
                    {
                        success = false,
                        message = "Validation failed!",
                        errors
                    };

                    return new BadRequestObjectResult(Result.Failure(errors, "Validation failed!"));
                };
            });

            return services;
        }
        #endregion
    }
}
