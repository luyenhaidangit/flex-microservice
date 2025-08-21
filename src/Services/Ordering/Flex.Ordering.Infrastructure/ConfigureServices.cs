using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common;
using Flex.Ordering.Application.Common.Interfaces;
using Flex.Ordering.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Flex.Infrastructure.EntityFrameworkCore.Oracle;
using Flex.Infrastructure.Common.Repositories;
using Flex.Ordering.Infrastructure.Repositories;
namespace Flex.Ordering.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureServiceDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.ConfigureServiceDbContext<OrderingDbContext>(configuration);

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            return services.AddScoped(typeof(IRepositoryBase<,,>), typeof(RepositoryBase<,,>))
                           .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>))
                           .AddScoped<IOrderRepository, OrderRepository>();
        }
    }
}
