using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Ordering.Application.Common.Interfaces;
using Flex.Ordering.Domain.Entities;
using Flex.Ordering.Infrastructure.Persistence;

namespace Flex.Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order, long, OrderingDbContext>, IOrderRepository
    {
        public OrderRepository(OrderingDbContext dbContext, IUnitOfWork<OrderingDbContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public Task<Order> GetOrderByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderByInvestorIdAsync(long investorId)
        {
            throw new NotImplementedException();
        }
    }
}
