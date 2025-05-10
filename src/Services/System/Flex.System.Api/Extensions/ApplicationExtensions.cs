using Flex.Infrastructure.Extensions;
using Flex.System.Api.Services;
using Serilog;

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

            app.UseApplicationMiddleware();

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
