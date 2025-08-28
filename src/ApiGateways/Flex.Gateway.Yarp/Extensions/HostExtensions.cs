using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Flex.Gateway.Yarp.Extensions
{
    public static class HostExtensions
    {
        public static void AddAppConfigurations(this WebApplicationBuilder builder)
        {
            var env = builder.Environment;

            // Add application configurations from JSON files and environment variables
            builder.Configuration
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            // Configure Kestrel for gRPC and HTTP/2
            builder.WebHost.ConfigureKestrel(options =>
            {
                // HTTP/2 for gRPC on port 5000
                options.ListenAnyIP(5000, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });

                // HTTPS on port 5001
                options.ListenAnyIP(5001, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    listenOptions.UseHttps();
                });

                // Configure limits for better performance
                options.Limits.MaxConcurrentConnections = 1000;
                options.Limits.MaxConcurrentUpgradedConnections = 1000;
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
                options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

                // Configure HTTP/2 settings
                options.Limits.Http2.MaxStreamsPerConnection = 100;
                options.Limits.Http2.HeaderTableSize = 4096;
                options.Limits.Http2.MaxFrameSize = 16384;
                options.Limits.Http2.MaxRequestHeaderFieldSize = 8192;
            });
        }
    }
}
