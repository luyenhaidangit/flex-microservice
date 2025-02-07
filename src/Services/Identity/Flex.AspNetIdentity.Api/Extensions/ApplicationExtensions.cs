using Serilog;
using Flex.Infrastructure.Extensions;

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

            app.UseApplicationMiddleware();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.MapControllers();
        }
    }
}
