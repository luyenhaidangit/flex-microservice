using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Flex.Infrastructure.Json
{
    public static class JsonExtensions
    {
        public static IServiceCollection ConfigureJsonOptionsDefault(this IServiceCollection services)
        {
            services.AddSingleton<IConfigureOptions<JsonOptions>, JsonOptionsSetup>();
            return services;
        }
    }
}
