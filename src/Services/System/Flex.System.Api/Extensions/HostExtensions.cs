using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Flex.System.Api.Extensions
{
    public static class HostExtensions
    {
        public static void AddAppConfigurations(this WebApplicationBuilder builder)
        {
            var env = builder.Environment;

            //  Adds application configurations from JSON files and environment variables.
            builder.Configuration
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            // Configure Kestrel for gRPC
            builder.WebHost.ConfigureKestrel(options =>
            {
                // HTTP/2 for gRPC
                var grpcPort = builder.Configuration.GetValue<int>("Grpc:Port", 5005);
                options.ListenAnyIP(grpcPort, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
        }
    }
}
