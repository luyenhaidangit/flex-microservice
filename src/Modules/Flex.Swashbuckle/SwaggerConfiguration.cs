using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Flex.Shared.Options;
using System.Reflection;

namespace Flex.Swashbuckle
{
    public static class SwaggerConfiguration
    {
        public static void ConfigureSwagger(this IServiceCollection services, ApiConfiguration configuration)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<LowerCaseDocumentFilter>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = $"{configuration.ApiName} Api",
                        Version = configuration.ApiVersion,
                    });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri($"{configuration.IdentityServerBaseUrl}/connect/authorize"),
                            Scopes = new Dictionary<string, string>
                            {
                                { $"{configuration.ApiName}.read", $"{configuration.ApiName} Read Scope" },
                                { $"{configuration.ApiName}.write", $"{configuration.ApiName} Write Scope" }
                            }
                        }
                    }
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer"
                        },
                        new List<string> { $"{configuration.ApiName}.read", $"{configuration.ApiName}.write" }
                    }
                });
            });
        }
    }
}
