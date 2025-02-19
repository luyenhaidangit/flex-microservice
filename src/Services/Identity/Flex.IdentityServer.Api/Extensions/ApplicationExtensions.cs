using Flex.Infrastructure.Extensions;
using Serilog;

namespace Flex.IdentityServer.Api.Extensions
{
    public static class ApplicationExtensions
    {
        public static async void UseInfrastructure(this WebApplication app)
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

            //app.UseApplicationMiddleware();

            app.UseIdentityServer();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseCors();

            app.MapControllers();
        }
    }
}
