using Flex.Infrastructure.Middlewares;
using Serilog;
using Flex.System.Api.Grpc;

namespace Flex.System.Api.Extensions
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

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            // Grpc
            app.MapGrpcService<BranchGrpcService>();

            app.MapControllers();
        }
    }
}
