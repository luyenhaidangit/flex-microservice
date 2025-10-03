using Flex.Infrastructure.Middlewares;
using Serilog;

namespace Flex.Workflow.Api.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}

