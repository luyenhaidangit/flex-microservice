using Duende.IdentityServer.EntityFramework.DbContexts;
using Flex.Infrastructure.Middlewares;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Flex.Identity.Api.Extensions
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

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseIdentityServer();

            using (var scope = app.Services.CreateScope())
            {
                var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                await configContext.Database.MigrateAsync();

                var persistedGrantContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
                await persistedGrantContext.Database.MigrateAsync();
            }


            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();

            app.UseCors();

            app.MapControllers();
        }
    }
}
