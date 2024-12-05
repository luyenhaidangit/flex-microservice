using Serilog;
using Flex.Common.Logging;
using Flex.Product.Api.Extensions;
using Flex.Product.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var host = builder.Host;
var configuration = builder.Configuration;
var environment = builder.Environment;

SeriLogger.Configure(configuration,environment);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    host.AddAppConfigurations();

    services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    app.UseInfrastructure();

    app.MigrateDatabase<ProductContext>((context, _) =>
    {
        ProductContextSeed.SeedProductAsync(context, Log.Logger).Wait();
    })
       .Run();

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
