using Flex.Basket.Api.Entities;
using Flex.Basket.Api.Repositories.Interfaces;
using Flex.Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using ILogger = Serilog.ILogger;

namespace Flex.Basket.Api.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;
        private readonly ILogger _logger;

        public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger logger)
        {
            _redisCacheService = redisCacheService;
            _serializeService = serializeService;
            _logger = logger;
        }

        public async Task<Cart?> GetBasketByUserName(string userName)
        {
            var basket = await _redisCacheService.GetStringAsync(userName);
            return string.IsNullOrEmpty(basket) ? null : _serializeService.Deserialize<Cart>(basket);
        }

        public async Task<Cart> UpdateBasket(Cart cart, DistributedCacheEntryOptions options = null)
        {
            if (options != null)
                await _redisCacheService.SetStringAsync(cart.Username,
                    _serializeService.Serialize(cart), options);
            else
                await _redisCacheService.SetStringAsync(cart.Username,
                    _serializeService.Serialize(cart));

            return await GetBasketByUserName(cart.Username);
        }

        public async Task<bool> DeleteBasketFromUserName(string username)
        {
            try
            {
                await _redisCacheService.RemoveAsync(username);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Error DeleteBasketFromUserName: " + e.Message);
                throw;
            }
        }
    }
}
