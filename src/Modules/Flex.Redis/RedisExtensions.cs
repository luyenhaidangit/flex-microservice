using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Flex.Redis
{
    public static class RedisExtensions
    {
        public static IServiceCollection ConfigureStackExchangeRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisConnection");
            });

            return services;
        }
    }
}
