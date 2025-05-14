namespace Flex.Data
{
    public interface IConnectionStringChecker
    {
        Task<ConnectionStringCheckResult> CheckAsync(string connectionString);
    }
}
