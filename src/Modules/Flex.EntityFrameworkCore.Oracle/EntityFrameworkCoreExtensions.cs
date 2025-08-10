using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;

namespace Flex.EntityFrameworkCore.Oracle
{
    public static class EntityFrameworkCoreExtensions
    {
        public static IServiceCollection ConfigureServiceDbContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            bool useWallet = false)
            where TContext : DbContext
        {
            OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;

            string connectionString = string.Empty;

            if (useWallet)
            {
                var walletConfig = configuration.GetSection("OracleWallet").Get<OracleWalletConfiguration>();

                if (walletConfig == null)
                {
                    throw new InvalidOperationException("OracleWallet configuration section is missing.");
                }

                if (!Path.IsPathRooted(walletConfig.WalletPath))
                {
                    var basePath = AppContext.BaseDirectory;
                    walletConfig.WalletPath = Path.GetFullPath(Path.Combine(basePath, walletConfig.WalletPath));
                }

                connectionString = new OracleConnectionStringBuilder
                {
                    UserID = walletConfig.User,
                    Password = walletConfig.Password,
                    DataSource = walletConfig.DataSource,
                    TnsAdmin = walletConfig.WalletPath,
                    WalletLocation = walletConfig.WalletPath,
                    PersistSecurityInfo = true,
                    Pooling = true,
                    ValidateConnection = true,
                    ConnectionTimeout = 15
                }.ToString();
            }
            else
            {
                connectionString = configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("DefaultConnection string is missing or empty. Please define it in appsettings.json under ConnectionStrings.");
                }
            }

            services.AddDbContext<TContext>(options =>
                options.UseOracle(connectionString));

            return services;
        }
    }
}
