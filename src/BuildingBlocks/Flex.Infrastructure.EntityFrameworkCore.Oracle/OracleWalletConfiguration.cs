namespace Flex.Infrastructure.EntityFrameworkCore.Oracle
{
    public class OracleWalletConfiguration
    {
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DataSource { get; set; } = string.Empty;
        public string WalletPath { get; set; } = string.Empty;
    }
}
