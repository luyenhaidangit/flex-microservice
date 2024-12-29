using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Flex.Shared.SeedWork;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.Common;
using Flex.Investor.Api.Persistence;
using Flex.Investor.Api.Repositories.Interfaces;
using Flex.Investor.Api.Repositories;
using Flex.Investor.Api.Services.Interfaces;
using Flex.Investor.Api.Services;
using Flex.Infrastructure.Swashbuckle;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Flex.Investor.Api.Extensions
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
            services.ConfigureInvestorDbContext(configuration);

            // AutoMapper
            services.ConfigureAutoMapper();

            // Infrastructure
            services.AddInfrastructureServices();

            // Response
            services.ConfigureValidationErrorResponse();

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

        private static IServiceCollection ConfigureInvestorDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;

            services.AddDbContext<InvestorDbContext>(options =>
            options.UseOracle(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                           .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                           .AddScoped<IInvestorRepository, InvestorRepository>()
                           .AddScoped<IInvestorService, InvestorService>();
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

        private static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AssemblyReference.Assembly);

            return services;
        }
        #endregion
    }
}
