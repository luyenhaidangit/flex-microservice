using Flex.Gateway.Yarp.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure host and Kestrel
builder.ConfigureGatewayHost();

// Add services to the container
builder.Services.AddGatewayServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseGatewayPipeline();

// Start the application
try
{
    Log.Information("Starting Flex.Gateway.Yarp");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
