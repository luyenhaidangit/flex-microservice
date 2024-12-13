using Flex.Common.Logging;
using Flex.Customer.Api.Controllers;
using Flex.Customer.Api.Extensions;
using Flex.Customer.Api;
using Serilog;
using Flex.Customer.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var host = builder.Host;
var configuration = builder.Configuration;
var environment = builder.Environment;

SeriLogger.Configure(builder);
Log.Information($"Start {builder.Environment.ApplicationName} up");

try
{
    builder.Host.AddAppConfigurations();
    // Add services to the container.
    builder.Services.AddConfigurationSettings(builder.Configuration);
    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddAutoMapper(AssemblyReference.Assembly);

    builder.Services.ConfigureCustomerContext();
    builder.Services.AddInfrastructureServices();
    //builder.Services.ConfigureHealthChecks();

    var app = builder.Build();

    app.MapGet("/", () => $"Welcome to {builder.Environment.ApplicationName}!");

    app.MapCustomersAPI();

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json",
            $"{builder.Environment.ApplicationName} v1"));
    });
    //}

    //app.UseMiddleware<ErrorWrappingMiddleware>();

    // app.UseHttpsRedirection(); //production only
    app.UseRouting();
    app.UseAuthorization();

    //app.UseEndpoints(endpoints =>
    //{
    //    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
    //    {
    //        Predicate = _ => true,
    //        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    //    });
    //    endpoints.MapDefaultControllerRoute();
    //});

    app.SeedCustomerData()
        .Run();
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