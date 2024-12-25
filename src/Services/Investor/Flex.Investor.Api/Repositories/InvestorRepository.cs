using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Investor.Api.Persistence;
using Flex.Investor.Api.Repositories.Interfaces;

namespace Flex.Investor.Api.Repositories
{
    public class InvestorRepository : RepositoryBase<Entities.Investor, long, InvestorDbContext>, IInvestorRepository
    {
        public InvestorRepository(InvestorDbContext dbContext, IUnitOfWork<InvestorDbContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public IQueryable<Entities.Investor> GetSampleData()
        {
            var sampleInvestors = new List<Entities.Investor>
            {
                new Entities.Investor { Id = 1, No = "INV001", FullName = "John Doe", Email = "john@example.com", Phone = "1234567890" },
                new Entities.Investor { Id = 2, No = "INV002", FullName = "Jane Smith", Email = "jane@example.com", Phone = "0987654321" },
                new Entities.Investor { Id = 3, No = "INV003", FullName = "Alice Johnson", Email = "alice@example.com", Phone = "1122334455" },
                new Entities.Investor { Id = 4, No = "INV004", FullName = "Bob Brown", Email = "bob@example.com", Phone = "6677889900" },
                new Entities.Investor { Id = 5, No = "INV005", FullName = "Charlie Davis", Email = "charlie@example.com", Phone = "5544332211" }
            };

            return sampleInvestors.AsQueryable();
        }
    }
}
