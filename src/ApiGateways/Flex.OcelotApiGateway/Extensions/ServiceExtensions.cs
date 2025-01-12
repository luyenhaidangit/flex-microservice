using Flex.Shared.Options;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Flex.OcelotApiGateway.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //// Bind configuration settings
            //var apiConfiguration = configuration.GetSection("ApiConfiguration").Get<ApiConfiguration>() ?? throw new InvalidOperationException("ApiConfiguration is missing or invalid in appsettings.json.");

            //// Add services to the container.
            //services.AddControllers(options =>
            //{
            //})
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //    options.JsonSerializerOptions.WriteIndented = true;
            //});

            //services.Configure<RouteOptions>(options =>
            //{
            //    options.LowercaseUrls = true;
            //    options.AppendTrailingSlash = false;
            //});

            //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //services.AddEndpointsApiExplorer();

            //// Configure Swagger
            //services.ConfigureSwagger(apiConfiguration);

            //// Database
            //services.ConfigureProductDbContext(configuration);

            //// AutoMapper
            //services.ConfigureAutoMapper();

            //// Infrastructure
            //services.AddInfrastructureServices();

            //// Response
            //services.ConfigureValidationErrorResponse();

            //// Cors
            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(policy =>
            //    {
            //        policy.WithOrigins("https://localhost:7179")
            //              .AllowAnyHeader()
            //              .AllowAnyMethod()
            //              .AllowCredentials();
            //    });
            //});

            //// SignIR
            //services.AddSignalR();

            return services;
        }

        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }
    }
}
