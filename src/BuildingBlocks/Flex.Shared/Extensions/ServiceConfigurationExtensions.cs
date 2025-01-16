using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Flex.Shared.SeedWork;

namespace Flex.Shared.Extensions
{
    public static class ServiceConfigurationExtensions
    {
        //RouteOptions
        public static IServiceCollection ConfigureRouteOptions(this IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = false;
            });

            return services;
        }

        //JsonSerializerOptions
        public static void ApplyJsonSettings(this JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.WriteIndented = true;
        }

        //GetRequiredConfiguration
        public static T GetRequiredConfiguration<T>(this IConfiguration configuration, string sectionName) where T : class, new()
        {
            var config = configuration.GetSection(sectionName).Get<T>();
            if (config == null)
            {
                throw new InvalidOperationException($"{sectionName} is missing or invalid in appsettings.json.");
            }
            return config;
        }

        //ConfigureValidationErrorResponse
        public static IServiceCollection ConfigureValidationErrorResponse(this IServiceCollection services)
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
    }
}
