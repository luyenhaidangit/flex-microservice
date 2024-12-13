using Serilog;
using Serilog.Sinks.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

namespace Flex.Common.Logging
{
    public static class SeriLogger
    {
        public static void Configure(WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            var hostingEnvironment = builder.Environment;
            var host = builder.Host;

            var applicationName = hostingEnvironment.ApplicationName?.ToLowerInvariant().Replace('.', '-') ?? "unknown-application";
            var environmentName = hostingEnvironment.EnvironmentName ?? "Development";
            var elasticUri = configuration["ElasticConfiguration:Uri"];
            var username = configuration["ElasticConfiguration:Username"];
            var password = configuration["ElasticConfiguration:Password"];

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Debug(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
                .WriteTo.File(
                    path: "logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    shared: true,
                    retainedFileCountLimit: 7
                 )
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
                {
                    IndexFormat = $"flex-{applicationName}-{environmentName}-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfReplicas = 1,
                    NumberOfShards = 2,
                    ModifyConnectionSettings = x => x.BasicAuthentication(username, password),
                })
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", environmentName)
                .Enrich.WithProperty("Application", applicationName)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            host.UseSerilog();
        }
    }
}
