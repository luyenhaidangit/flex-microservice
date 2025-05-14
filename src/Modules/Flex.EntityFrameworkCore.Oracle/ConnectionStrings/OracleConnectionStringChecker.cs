using Oracle.ManagedDataAccess.Client;
using Flex.Data;
using Flex.Core.DependencyInjection;

namespace Flex.EntityFrameworkCore.Oracle.ConnectionStrings
{
    [Dependency(ReplaceServices = true)]
    public class OracleConnectionStringChecker : IConnectionStringChecker, ITransientDependency
    {
        public virtual async Task<ConnectionStringCheckResult> CheckAsync(string connectionString)
        {
            var result = new ConnectionStringCheckResult();
            try
            {
                var connString = new OracleConnectionStringBuilder(connectionString)
                {
                    ConnectionTimeout = 1
                };

                await using var conn = new OracleConnection(connString.ConnectionString);
                await conn.OpenAsync();
                result.Connected = true;
                result.DatabaseExists = true;

                await conn.CloseAsync();

                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
    }
}
