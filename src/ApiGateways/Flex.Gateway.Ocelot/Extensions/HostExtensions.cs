namespace Flex.Gateway.Ocelot.Extensions
{
    public static class HostExtensions
    {
        public static void AddAppConfigurations(this WebApplicationBuilder builder)
        {
            var env = builder.Environment;

            //  Adds application configurations from JSON files and environment variables.
            builder.Configuration
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"ocelot.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();
        }
    }
}
