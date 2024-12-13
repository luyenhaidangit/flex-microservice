using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;
using Flex.Securities.Api.Persistence;
using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.Common;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Securities.Api.Repositories;
using Flex.Common.Documentation;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Flex.Securities.Api.Extensions
{
    public static class ServiceExtensions
    {
        #region Config infrastructure service
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.ConfigureSwagger();

            // Database
            services.ConfigureProductDbContext(configuration);

            // AutoMapper
            services.ConfigureAutoMapper();

            // Infrastructure
            services.AddInfrastructureServices();

            //services.AddControllers();
            //services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //services.AddEndpointsApiExplorer();
            //services.ConfigureSwagger();
            //services.ConfigureProductDbContext(configuration);
            //services.AddInfrastructureServices();
            //services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
            //// services.AddJwtAuthentication();
            //services.ConfigureAuthenticationHandler();
            //services.ConfigureAuthorization();
            //services.ConfigureHealthChecks();
            return services;
        }

        private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<LowerCaseDocumentFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        private static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AssemblyReference.Assembly);

            return services;
        }

        private static IServiceCollection ConfigureProductDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;

            services.AddDbContext<SecuritiesDbContext>(options =>
            options.UseOracle(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                           .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                           .AddScoped<ISecuritiesRepository, SecuritiesRepository>()
                           .AddScoped<IIssuerRepository,IssuerRepository >();
        }
        #endregion

        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        //internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        //{
        //    var settings = services.GetOptions<JwtSettings>(nameof(JwtSettings));
        //    if (settings == null || string.IsNullOrEmpty(settings.Key))
        //        throw new ArgumentNullException($"{nameof(JwtSettings)} is not configured properly");

        //    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = signingKey,
        //        ValidateIssuer = false,
        //        ValidateAudience = false,
        //        ValidateLifetime = false,
        //        ClockSkew = TimeSpan.Zero,
        //        RequireExpirationTime = false
        //    };
        //    services.AddAuthentication(o =>
        //    {
        //        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    }).AddJwtBearer(x =>
        //    {
        //        x.SaveToken = true;
        //        x.RequireHttpsMetadata = false;
        //        x.TokenValidationParameters = tokenValidationParameters;
        //    });

        //    return services;
        //}

        //private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        //{
        //    return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
        //            .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
        //            .AddScoped<IProductRepository, ProductRepository>()
        //        ;
        //}

        //private static void ConfigureHealthChecks(this IServiceCollection services)
        //{
        //    var databaseSettings = services.GetOptions<DatabaseSettings>(nameof(DatabaseSettings));
        //    services.AddHealthChecks()
        //        .AddMySql(databaseSettings.ConnectionString, "MySql Health", HealthStatus.Degraded);
        //}

        //public static void ConfigureSwagger(this IServiceCollection services)
        //{
        //    var configuration = services.GetOptions<ApiConfiguration>("ApiConfiguration");
        //    if (configuration == null || string.IsNullOrEmpty(configuration.IssuerUri) ||
        //        string.IsNullOrEmpty(configuration.ApiName)) throw new Exception("ApiConfiguration is not configured!");

        //    services.AddSwaggerGen(c =>
        //    {
        //        c.SwaggerDoc("v1",
        //            new OpenApiInfo
        //            {
        //                Title = "Product API V1",
        //                Version = configuration.ApiVersion,
        //            });

        //        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //        {
        //            Type = SecuritySchemeType.OAuth2,
        //            Flows = new OpenApiOAuthFlows
        //            {
        //                Implicit = new OpenApiOAuthFlow
        //                {
        //                    AuthorizationUrl = new Uri($"{configuration.IdentityServerBaseUrl}/connect/authorize"),
        //                    Scopes = new Dictionary<string, string>
        //                    {
        //                        { "tedu_microservices_api.read", "Tedu Microservices API Read Scope" },
        //                        { "tedu_microservices_api.write", "Tedu Microservices API Write Scope" }
        //                    }
        //                }
        //            }
        //        });
        //        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        //        {
        //            {
        //                new OpenApiSecurityScheme
        //                {
        //                    Reference = new OpenApiReference
        //                    {
        //                        Type = ReferenceType.SecurityScheme,
        //                        Id = "Bearer"
        //                    },
        //                    Name = "Bearer"
        //                },
        //                new List<string>
        //                {
        //                    "tedu_microservices_api.read",
        //                    "tedu_microservices_api.write"
        //                }
        //            }
        //        });
        //    });

        //}
    }
}
