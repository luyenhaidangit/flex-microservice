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
            catch (BadRequestException ex)
            {
                _logger.LogError(ex, "Bad request exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError, ex.Message);
            }

            if (!context.Response.HasStarted)
            {
                await HandleCustomResponseAsync(context);
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

        private async Task HandleCustomResponseAsync(HttpContext context)
        {
            var response = Result.Failure("Internal Server");

            switch (context.Response.StatusCode)
            {
                case StatusCodes.Status401Unauthorized:
                case StatusCodes.Status403Forbidden:
                    response = Result.Failure("Unauthorized");
                    break;

                case StatusCodes.Status429TooManyRequests:
                    response = Result.Failure("Too many requests");
                    break;

                default:
                    return;
            }

            context.Response.ContentType = "application/json";
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var responseJson = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(responseJson);
        }
    }
}
