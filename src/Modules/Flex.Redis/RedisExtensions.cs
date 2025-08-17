using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Flex.Infrastructure.Redis
{
    public static class RedisExtensions
    {
        public static IServiceCollection ConfigureStackExchangeRedisCache(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind RedisOptions settings
            var redisOptions = new RedisOptions();
            var section = configuration.GetSection(RedisConstants.RedisOptionsSection);
            if (section.Exists()) section.Bind(redisOptions);

            var endpoint = configuration.GetConnectionString(RedisConstants.RedisConnectionKey);
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new InvalidOperationException($"Missing connection string '{RedisConstants.RedisConnectionKey}'.");
            }

            // Set minimum thread pool size to ensure Redis operations can run efficiently
            ThreadPool.GetMinThreads(out var worker, out var iocp);
            var desired = Math.Max(worker, Environment.ProcessorCount * 2);
            if (desired > worker) ThreadPool.SetMinThreads(desired, iocp);

            // Configure Redis connection options
            var cfg = ConfigurationOptions.Parse(endpoint, false);
            cfg.ClientName = AppDomain.CurrentDomain.FriendlyName;
            cfg.AbortOnConnectFail = redisOptions.AbortOnConnectFail;
            cfg.KeepAlive = redisOptions.KeepAliveSeconds;
            cfg.ConnectTimeout = redisOptions.ConnectTimeoutMs;
            cfg.AsyncTimeout = redisOptions.AsyncTimeoutMs;
            cfg.ConnectRetry = redisOptions.ConnectRetry;
            cfg.ReconnectRetryPolicy = new ExponentialRetry(redisOptions.ReconnectBaseMs);

            // Optional: Configure additional Redis options if provided
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("Redis");
                var mux = ConnectionMultiplexer.Connect(cfg);

                mux.ConnectionFailed += (_, e) => logger.LogWarning(e.Exception, "Redis connection failed: {Type} {EndPoint}", e.FailureType, e.EndPoint);
                mux.ConnectionRestored += (_, e) => logger.LogInformation("Redis connection restored: {Type} {EndPoint}", e.FailureType, e.EndPoint);
                mux.ErrorMessage += (_, e) => logger.LogWarning("Redis error: {Message}", e.Message);

                return mux;
            });

            // Register Redis cache with the configured multiplexer
            services.AddStackExchangeRedisCache(_ => {});

            services.AddOptions<RedisCacheOptions>()
                    .Configure<IConnectionMultiplexer>((o, mux) =>
                    {
                        o.ConnectionMultiplexerFactory = () => Task.FromResult(mux);
                        if (!string.IsNullOrWhiteSpace(redisOptions.InstanceName))
                            o.InstanceName = redisOptions.InstanceName!;
                    });

            return services;
        }
    }
}
