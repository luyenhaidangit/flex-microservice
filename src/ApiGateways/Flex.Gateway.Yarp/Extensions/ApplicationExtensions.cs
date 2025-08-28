using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using System.Threading.RateLimiting;
using Flex.Gateway.Yarp.Middleware;

namespace Flex.Gateway.Yarp.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseGatewayPipeline(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            // Use error handling middleware
            app.UseGatewayErrorHandling();

            // Use Serilog for request logging
            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
                    diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
                    diagnosticContext.Set("User", httpContext.User?.Identity?.Name ?? "anonymous");
                };
            });

            // Use HTTPS redirection
            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });

            // Use Rate Limiting
            app.UseRateLimiter();

            // Use Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Use Routing
            app.UseRouting();

            // Map Health Checks
            app.MapHealthChecks("/health", new()
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var result = new
                    {
                        status = report.Status.ToString(),
                        checks = report.Entries.Select(entry => new
                        {
                            name = entry.Key,
                            status = entry.Value.Status.ToString(),
                            description = entry.Value.Description
                        })
                    };
                    await context.Response.WriteAsJsonAsync(result);
                }
            });

            // Map YARP Reverse Proxy
            app.MapReverseProxy();

            // Map fallback for unmatched routes
            app.MapFallback(async context =>
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Route not found",
                    message = "The requested route does not exist in the gateway",
                    path = context.Request.Path,
                    timestamp = DateTime.UtcNow
                });
            });
        }
    }
}
