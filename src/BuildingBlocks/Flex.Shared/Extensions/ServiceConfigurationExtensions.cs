using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        //GetRequiredConfiguration
        public static T GetRequiredSection<T>(this IConfiguration configuration, string sectionName) where T : class, new()
        {
            var config = configuration.GetSection(sectionName).Get<T>();
            if (config == null)
            {
                throw new InvalidOperationException($"{sectionName} is missing or invalid in appsettings.json.");
            }
            return config;
        }

        public static T GetRequiredValue<T>(this IConfiguration configuration, string key)
        {
            var value = configuration.GetValue<T>(key);

            if (EqualityComparer<T>.Default.Equals(value, default))
            {
                throw new InvalidOperationException($"Key '{key}' is missing or invalid in appsettings.json.");
            }

            return value;
        }

        public static IMvcBuilder ApplyJsonSettings(this IMvcBuilder builder)
        {
            return builder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                //options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;  // .NET 5+
                options.JsonSerializerOptions.WriteIndented = true;
            });
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
