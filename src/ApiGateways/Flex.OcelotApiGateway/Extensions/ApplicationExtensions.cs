using Flex.Infrastructure.Middlewares;
using Flex.OcelotApiGateway.Constants;
using Ocelot.Middleware;
using Serilog;

namespace Flex.OcelotApiGateway.Extensions
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

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

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
