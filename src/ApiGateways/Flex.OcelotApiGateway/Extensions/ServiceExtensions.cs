using System.Text;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;
using Ocelot.Cache.CacheManager;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Flex.Shared.Constants;
using Flex.Shared.Configurations;

namespace Flex.OcelotApiGateway.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ApiConfiguration>().Bind(configuration.GetSection(ConfigurationConstants.ApiConfigurationSection)).ValidateDataAnnotations().ValidateOnStart();
            services.AddOptions<JwtSettings>().Bind(configuration.GetSection(ConfigurationConstants.JwtSettingsSection)).ValidateDataAnnotations().ValidateOnStart();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register services DI container
            services.AddInfrastructureServices();

            // Configure Ocelot API gateway
            services.ConfigureOcelot(configuration);

            // Add Jwt authentication
            services.AddJwtAuthentication(configuration);

            //// Bind configuration settings
            //var apiConfiguration = configuration.GetSection("ApiConfiguration").Get<ApiConfiguration>() ?? throw new InvalidOperationException("ApiConfiguration is missing or invalid in appsettings.json.");

            //// Add services to the container.
            //services.AddControllers(options =>
            //{
            //})
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            //    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            //    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            //    options.JsonSerializerOptions.WriteIndented = true;
            //});

            //services.Configure<RouteOptions>(options =>
            //{
            //    options.LowercaseUrls = true;
            //    options.AppendTrailingSlash = false;
            //});

            //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //services.AddEndpointsApiExplorer();

            //// Configure Swagger
            //services.ConfigureSwagger(apiConfiguration);

            //// Database
            //services.ConfigureProductDbContext(configuration);

            //// AutoMapper
            //services.ConfigureAutoMapper();

            //// Infrastructure
            //services.AddInfrastructureServices();

            //// Response
            //services.ConfigureValidationErrorResponse();

            //// Cors
            //services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(policy =>
            //    {
            //        policy.WithOrigins("https://localhost:7179")
            //              .AllowAnyHeader()
            //              .AllowAnyMethod()
            //              .AllowCredentials();
            //    });
            //});

            //// SignIR
            //services.AddSignalR();

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            //services.AddTransient<ITokenService, TokenService>();

            return services;
        }

        private static void ConfigureOcelot(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOcelot(configuration)
                    .AddPolly()
                    .AddCacheManager(x => x.WithDictionaryHandle());

            services.AddSwaggerForOcelot(configuration, x =>
            {
                x.GenerateDocsForGatewayItSelf = false;
            });
        }

        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(ConfigurationConstants.JwtSettingsSection).Get<JwtSettings>();

            if (settings == null || string.IsNullOrEmpty(settings.SecretKey))
            {
                throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured properly");
            }
                
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.SecretKey));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = false
            };
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            return services;
        }
        #endregion
    }
}
