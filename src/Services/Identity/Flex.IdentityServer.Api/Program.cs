using Serilog;
using Flex.SeriLog;
using Flex.IdentityServer.Api.Extensions;
using Flex.IdentityServer.Api.Persistence;

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

    await app.MigrateDatabase<IdentityDbContext>(async (context, services) =>
    {
        await IdentityDbSeed.InitAsync(services, Log.Logger);
    });

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
