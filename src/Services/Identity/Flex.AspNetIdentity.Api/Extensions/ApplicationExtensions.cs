using Serilog;
using Flex.Infrastructure.Extensions;
using Flex.Infrastructure.Middlewares;

namespace Flex.AspNetIdentity.Api.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Logging
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseApplicationMiddleware();
            //app.UseMiddleware<ErrorWrappingMiddleware>();

            app.MapControllers();
        }
    }
}
