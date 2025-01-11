using Flex.Infrastructure.Middlewares;
using Serilog;

namespace Flex.Identity.Api.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DisplayRequestDuration();
                });
            }

            // Logging
            app.UseSerilogRequestLogging();

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseCors();

            app.MapControllers();
        }
    }
}
