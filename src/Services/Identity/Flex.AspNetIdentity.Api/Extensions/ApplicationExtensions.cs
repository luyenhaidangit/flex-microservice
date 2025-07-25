﻿using Flex.Infrastructure.Middlewares;
using Serilog;

namespace Flex.AspNetIdentity.Api.Extensions
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

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
        }
    }
}
