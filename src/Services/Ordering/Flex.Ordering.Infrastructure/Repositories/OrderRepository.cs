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
    }
}
