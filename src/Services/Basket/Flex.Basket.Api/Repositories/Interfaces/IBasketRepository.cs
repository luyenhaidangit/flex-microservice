using Flex.Basket.Api.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Flex.Basket.Api.Repositories.Interfaces
{
    public interface IBasketRepository
    {
        Task<Cart?> GetBasketByInvestorIdAsync(string investorId);
        Task<Cart> UpdateBasketAsync(Cart cart, DistributedCacheEntryOptions? options = null);
        Task<bool> DeleteBasketByInvestorIdAsync(string investorId);
    }
}
