using Flex.Infrastructure.Middlewares;
using Flex.OcelotApiGateway1.Constants;
using Serilog;

namespace Flex.OcelotApiGateway1.Extensions
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

            app.UseCors(GatewayConstants.CorsPolicy);

            app.MapControllers();
        }
    }
}
