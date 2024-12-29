using Serilog;
using Flex.SeriLog;
using Flex.Investor.Api.Extensions;
using Flex.Investor.Api.Controllers;
using Flex.Investor.Api.Persistence;

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

    // Endpoints
    app.MapInvestorsApi();

    app.UseInfrastructure();

    await app.SeedsDataAsync();

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