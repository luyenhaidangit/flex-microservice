using Flex.Common.Documentation;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json;
using Flex.Shared.SeedWork;

namespace Flex.Email.Runtime.Api.Extensions
{
    public static class ServiceExtensions
    {
        #region Infrastructure
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
            services.ConfigureProductDbContext(configuration);

            // AutoMapper
            services.ConfigureAutoMapper();

            // Infrastructure
            services.AddInfrastructureServices();

            // Response
            services.ConfigureValidationErrorResponse();

            return services;
        }

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
            services.AddAutoMapper(AssemblyReference.Assembly);

            return services;
        }

        private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            //OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;

            //services.AddDbContext<SecuritiesDbContext>(options =>
            //options.UseOracle(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection ConfigureNewtonsoftJson(this IServiceCollection services)
        {
            services.AddControllers();

            return services;
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

        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
