namespace Flex.Securities.Api.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            //app.UseSwagger();
            //app.UseSwaggerUI(c =>
            //{
            //    c.OAuthClientId("tedu_microservices_swagger");
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API");
            //    c.DisplayRequestDuration();
            //});
            //app.UseMiddleware<ErrorWrappingMiddleware>();
            //app.UseAuthentication();
            //app.UseRouting();
            //app.UseHttpsRedirection(); //for production only
            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            //    {
            //        Predicate = _ => true,
            //        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            //    });
            //    endpoints.MapDefaultControllerRoute();
            //});
        }
    }
}
