using Flex.Infrastructure.Middlewares;
using Flex.Gateway.Ocelot.Constants;
using Ocelot.Middleware;
using Serilog;

namespace Flex.Gateway.Ocelot.Extensions
{
    public static class ApplicationExtensions
    {
        public async static Task UseInfrastructure(this WebApplication app)
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

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseCors(GatewayConstants.CorsPolicy);

            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            await app.UseOcelot();

            app.MapControllers();
        }
    }
}
