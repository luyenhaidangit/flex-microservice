using Flex.Contracts.Domains.Interfaces;
using Flex.Ordering.Domain.Entities;

namespace Flex.Ordering.Application.Common.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order, long>
    {
        //Task<Order> GetOrderByIdAsync(long id);
        //Task<Order> GetOrderByInvestorIdAsync(long investorId);
        //Task<Order> GetOrderByDocumentNo(string documentNo);
        //void CreateOrder(Order order);
        //Task<Order> UpdateOrderAsync(Order order);
        //void DeleteOrder(Order order);
    }
}
