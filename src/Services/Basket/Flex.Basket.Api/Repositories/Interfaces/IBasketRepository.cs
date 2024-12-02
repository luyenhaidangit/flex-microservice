using Flex.Basket.Api.Entities;
using Microsoft.Extensions.Caching.Distributed;

namespace Flex.Basket.Api.Repositories.Interfaces
{
    public interface IBasketRepository
    {
        Task<Cart?> GetBasketByUserName(string userName);
        Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null);
        Task<bool> DeleteBasketFromUserName(string username);
    }
}
