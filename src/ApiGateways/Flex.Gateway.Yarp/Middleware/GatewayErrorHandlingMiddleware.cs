using System.Net;
using System.Text.Json;

namespace Flex.Gateway.Yarp.Middleware
{
    public class GatewayErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GatewayErrorHandlingMiddleware> _logger;

        public GatewayErrorHandlingMiddleware(RequestDelegate next, ILogger<GatewayErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request processing");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var (statusCode, message) = exception switch
            {
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized access"),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid request"),
                TimeoutException => (HttpStatusCode.GatewayTimeout, "Request timeout"),
                _ => (HttpStatusCode.InternalServerError, "An internal server error occurred")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                error = statusCode.ToString(),
                message = message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path,
                method = context.Request.Method,
                correlationId = context.TraceIdentifier
            };

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public static class GatewayErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGatewayErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GatewayErrorHandlingMiddleware>();
        }
    }
}
