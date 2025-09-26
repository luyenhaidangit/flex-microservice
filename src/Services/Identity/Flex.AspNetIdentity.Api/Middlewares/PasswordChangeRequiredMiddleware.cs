using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.Constants;
using Flex.Infrastructure.Exceptions;
using System.Security.Claims;

namespace Flex.AspNetIdentity.Api.Middlewares
{
    /// <summary>
    /// Middleware to enforce password change requirement for users who need to change password on first login
    /// </summary>
    public class PasswordChangeRequiredMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PasswordChangeRequiredMiddleware> _logger;

        public PasswordChangeRequiredMiddleware(RequestDelegate next, ILogger<PasswordChangeRequiredMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            // ===== Skip middleware for certain paths =====
            if (ShouldSkipMiddleware(context))
            {
                await _next(context);
                return;
            }

            // ===== Check if user is authenticated =====
            if (!context.User.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            // ===== Get username from claims =====
            var username = context.User.FindFirst(ClaimTypes.Name)?.Value ?? 
                          context.User.FindFirst("username")?.Value ??
                          context.User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(username))
            {
                await _next(context);
                return;
            }

            try
            {
                // ===== Check if password change is required =====
                var passwordChangeRequired = await userService.CheckPasswordChangeRequiredAsync(username);
                
                if (passwordChangeRequired)
                {
                    _logger.LogWarning("User {UserName} attempted to access API while password change is required", username);
                    
                    // ===== Return 403 Forbidden with specific error =====
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    
                    var response = new
                    {
                        success = false,
                        errorCode = "PASSWORD_CHANGE_REQUIRED",
                        message = "You must change your password before accessing the system",
                        data = (object?)null
                    };
                    
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
                    return;
                }
            }
            catch (ValidationException ex) when (ex.ErrorCode == ErrorCode.UserNotFound)
            {
                // User not found - let authentication handle this
                _logger.LogWarning("User {UserName} not found during password change check", username);
            }
            catch (Exception ex)
            {
                // Log error but don't block the request
                _logger.LogError(ex, "Error checking password change requirement for user {UserName}", username);
            }

            await _next(context);
        }

        /// <summary>
        /// Determine if middleware should be skipped for the current request
        /// </summary>
        private static bool ShouldSkipMiddleware(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

            // ===== Skip for authentication endpoints =====
            if (path.Contains("/auth/") || path.Contains("/login") || path.Contains("/logout"))
            {
                return true;
            }

            // ===== Skip for password change endpoint =====
            if (path.Contains("/users/change-password"))
            {
                return true;
            }

            // ===== Skip for password change required check endpoint =====
            if (path.Contains("/password-change-required"))
            {
                return true;
            }

            // ===== Skip for health checks =====
            if (path.Contains("/health") || path.Contains("/ping"))
            {
                return true;
            }

            // ===== Skip for static files =====
            if (path.Contains("/css/") || path.Contains("/js/") || path.Contains("/images/") || 
                path.Contains("/fonts/") || path.Contains("/favicon"))
            {
                return true;
            }

            // ===== Skip for Swagger/OpenAPI =====
            if (path.Contains("/swagger") || path.Contains("/openapi"))
            {
                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Extension methods for registering PasswordChangeRequiredMiddleware
    /// </summary>
    public static class PasswordChangeRequiredMiddlewareExtensions
    {
        /// <summary>
        /// Add password change required middleware to the pipeline
        /// </summary>
        public static IApplicationBuilder UsePasswordChangeRequired(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PasswordChangeRequiredMiddleware>();
        }
    }
}
