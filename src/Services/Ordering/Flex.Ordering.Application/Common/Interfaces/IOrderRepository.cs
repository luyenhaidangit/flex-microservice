using Flex.Contracts.Domains.Interfaces;
using Flex.Ordering.Domain.Entities;

namespace Flex.Ordering.Application.Common.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order, long>
    {
        Task<IEnumerable<Order>> GetOrdersByUserNameAsync(string userName);
        Task<Order> GetOrderByDocumentNo(string documentNo);
        void CreateOrder(Order order);
        Task<Order> UpdateOrderAsync(Order order);
        void DeleteOrder(Order order);
    }
}
