namespace Flex.Identity.Infrastructure.Domains
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> CommitAsync();
    }
}
