using Flex.Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Flex.Infrastructure.Common
{
    public class UnitOfWork<TContext> : IUnitOfWork<TContext> where TContext : DbContext
    {
        private readonly TContext _context;

        public UnitOfWork(TContext context)
        {
            _context = context;
        }

        public void Dispose() => _context.Dispose();

        public Task<int> CommitAsync() => _context.SaveChangesAsync();
    }
}
