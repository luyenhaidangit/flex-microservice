using Flex.Shared.Constants;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Flex.Infrastructure.Middlewares
{
    public class IpWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly List<string> _allowedIps;

        public IpWhitelistMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _allowedIps = configuration.GetSection(ConfigurationConstants.IpWhitelist).Get<List<string>>() ?? new List<string>();
        }

        public async Task Invoke(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress?.ToString();

            if (!_allowedIps.Contains(remoteIp))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = Result.Failure(message: "Access Denied.");

                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var responseJson = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(responseJson);
                return;
            }

            await _next(context);
        }
    }
}
