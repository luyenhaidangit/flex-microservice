using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Flex.Infrastructure.Exceptions;
using Flex.Shared.SeedWork;

namespace Flex.Infrastructure.Middlewares
{
    public class ErrorWrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorWrappingMiddleware> _logger;

        public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (ValidationException ex)
            {
                 //var errorMsg = ex.Errors.FirstOrDefault().Value.FirstOrDefault();
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, ex.Message);
            }

            if (!context.Response.HasStarted && 
                (context.Response.StatusCode == StatusCodes.Status401Unauthorized) ||
                context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";

                var response = new ApiErrorResult<bool>("Unauthorized");

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var responseJson = JsonSerializer.Serialize(Result.Failure(message), options);

            await context.Response.WriteAsync(responseJson);
        }
    }
}
