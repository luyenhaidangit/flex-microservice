using Microsoft.Extensions.DependencyInjection;

namespace Flex.Hangfire
{
    public static class HangfireExtensions
    {
        public static IServiceCollection AddTeduHangfireService(this IServiceCollection services)
        {
            //var settings = services.GetOptions<HangFireSettings>("HangFireSettings");
            //if (settings == null || settings.Storage == null ||
            //    string.IsNullOrEmpty(settings.Storage.ConnectionString))
            //    throw new Exception("HangFireSettings is not configured properly!");

            //services.ConfigureHangfireServices(settings);
            //services.AddHangfireServer(serverOptions
            //    => { serverOptions.ServerName = settings.ServerName; });

            return services;
        }

    }
}
