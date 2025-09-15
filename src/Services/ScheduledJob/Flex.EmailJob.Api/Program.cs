using Serilog;
using Flex.SeriLog;
using Flex.EmailJob.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.AddAppConfigurations();

SeriLogger.Configure(builder);
Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Services.AddConfigurationSettings(configuration);
    builder.Services.AddInfrastructure(configuration);

    var app = builder.Build();
    app.UseInfrastructure();

    app.Run();
}

catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shutdown {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}
