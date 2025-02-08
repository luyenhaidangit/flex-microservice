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
            if (this.IsMissingContentType(context))
            {
                await HandleExceptionAsync(context, StatusCodes.Status415UnsupportedMediaType, "Unsupported Media Type: Content-Type header is missing.");
                return;
            }

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
            var responseJson = JsonSerializer.Serialize(Result.Failure(message: message), options);

            await context.Response.WriteAsync(responseJson);
        }

        private async Task HandleCustomResponseAsync(HttpContext context)
        {
            var response = Result.Failure(message: "Internal Server");

            switch (context.Response.StatusCode)
            {
                case StatusCodes.Status401Unauthorized:
                    response = Result.Failure(message: "Unauthorized Access. Please provide a valid token.");
                    break;
                case StatusCodes.Status403Forbidden:
                    response = Result.Failure(message: "Unauthorized");
                    break;
                case StatusCodes.Status404NotFound:
                    response = Result.Failure(message: "Api Not Found");
                    break;
                case StatusCodes.Status415UnsupportedMediaType:
                    response = Result.Failure(message: "Unsupported Media Type");
                    break;
                case StatusCodes.Status429TooManyRequests:
                    response = Result.Failure(message: "Too many requests");
                    break;
                case StatusCodes.Status502BadGateway:
                    response = Result.Failure(message: "Service is unavailable or down. Please try again later.");
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

        private bool IsMissingContentType(HttpContext context)
        {
            if (context.Request.Method == HttpMethods.Post ||
                context.Request.Method == HttpMethods.Put ||
                context.Request.Method == HttpMethods.Patch)
            {
                return !context.Request.Headers.ContainsKey("Content-Type") ||
                       string.IsNullOrEmpty(context.Request.ContentType);
            }
            return false;
        }
    }
}
