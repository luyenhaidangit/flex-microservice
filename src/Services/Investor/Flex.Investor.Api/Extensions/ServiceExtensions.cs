using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.Common;
using Flex.Investor.Api.Persistence;
using Flex.Investor.Api.Repositories.Interfaces;
using Flex.Investor.Api.Repositories;
using Flex.Investor.Api.Services.Interfaces;
using Flex.Investor.Api.Services;
using Flex.Infrastructure.Swashbuckle;
using Flex.Shared.Extensions;
using Flex.Infrastructure.EntityFrameworkCore.Oracle;

namespace Flex.Investor.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddConfigurationSettings(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddControllers().ApplyJsonSettings();

            services.AddEndpointsApiExplorer();

            services.ConfigureSwagger();

            services.ConfigureRouteOptions();
            services.ConfigureValidationErrorResponse();

            // Register services DI container
            services.AddInfrastructureServices();

            // Database
            services.ConfigureServiceDbContext<InvestorDbContext>(configuration);

            // AutoMapper
            services.AddAutoMapper(AssemblyReference.Assembly);

            return services;
        }

        #region Infrastructure
        private static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                           .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                           .AddScoped<IInvestorRepository, InvestorRepository>()
                           .AddScoped<ISubAccountRepository, SubAccountRepository>()
                           .AddScoped<IInvestorService, InvestorService>();
        }
        #endregion
    }
}
