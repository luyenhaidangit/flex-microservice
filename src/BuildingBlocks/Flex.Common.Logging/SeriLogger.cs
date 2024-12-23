using Serilog;
using Serilog.Sinks.Elasticsearch;
using Microsoft.AspNetCore.Builder;

namespace Flex.Common.Logging
{
    public static class SeriLogger
    {
        private const string OutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

        public static void Configure(WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            var env = builder.Environment;
            var host = builder.Host;

            // Read application name and environment name
            var applicationName = env.ApplicationName?.ToLowerInvariant().Replace('.', '-') ?? "unknown-application";
            var environmentName = env.EnvironmentName ?? "Development";

            var elasticUri = configuration["ElasticConfiguration:Uri"];
            var username = configuration["ElasticConfiguration:Username"];
            var password = configuration["ElasticConfiguration:Password"];

            // Create logger configuration
            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Information()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", environmentName)
                .Enrich.WithProperty("Application", applicationName)
                .WriteTo.Debug(outputTemplate: OutputTemplate)
                .WriteTo.Console(outputTemplate: OutputTemplate)
                .WriteTo.File(
                    path: "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 7,
                    shared: true,
                    outputTemplate: OutputTemplate
                );

            // Add Elasticsearch sink if elasticUri is configured
            if (!string.IsNullOrWhiteSpace(elasticUri))
            {
                loggerConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    IndexFormat = $"flex-{applicationName}-{environmentName}-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfReplicas = 1,
                    NumberOfShards = 2,
                    ModifyConnectionSettings = c => c.BasicAuthentication(username, password),
                });
            }

            // Create logger
            Log.Logger = loggerConfig.CreateLogger();

            // Use Serilog
            host.UseSerilog();
        }
    }
}
