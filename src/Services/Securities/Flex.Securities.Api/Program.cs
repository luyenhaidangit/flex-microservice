using Serilog;
using Flex.Common.Logging;
using Flex.Securities.Api.Bootstraping.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.AddAppConfigurations();

SeriLogger.Configure(builder.Configuration, builder.Environment);
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
