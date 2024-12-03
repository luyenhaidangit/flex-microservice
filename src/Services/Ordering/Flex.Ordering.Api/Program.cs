using Serilog;
using Flex.Common.Logging;
using Flex.Ordering.Infrastructure.Persistence;
using Flex.Ordering.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var host = builder.Host;
var configuration = builder.Configuration;
var environment = builder.Environment;

SeriLogger.Configure(configuration, environment);

Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var orderContextSeed = scope.ServiceProvider.GetRequiredService<OrderContextSeed>();
        await orderContextSeed.InitialiseAsync();
        await orderContextSeed.SeedAsync();
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}

catch (Exception ex)
{
    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shutdown {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}
