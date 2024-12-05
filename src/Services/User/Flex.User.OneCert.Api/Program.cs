using Serilog;
using Flex.Common.Logging;

namespace Flex.User.OneCert.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            SeriLogger.Configure(builder.Configuration, builder.Environment);

            Log.Information($"Start {builder.Environment.ApplicationName} up");

            try
            {
                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

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
        }
    }
}
