using Flex.Basket.Api.Entities;
using Flex.Basket.Api.Repositories.Interfaces;
using Flex.Contracts.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace Flex.Basket.Api.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ILogger<BasketRepository> _logger;
        private readonly IDistributedCache _redisCacheService;
        private readonly ISerializeService _serializeService;

        public BasketRepository(IDistributedCache redisCacheService, ISerializeService serializeService, ILogger<BasketRepository> logger)
        {
            _logger = logger;
            _redisCacheService = redisCacheService;
            _serializeService = serializeService;    
        }

        public async Task<Cart?> GetBasketByInvestorIdAsync(string investorId)
        {
            _logger.LogInformation($"BEGIN: GetBasketByInvestorIdAsync {investorId}");
            var cachedData = await _redisCacheService.GetStringAsync(investorId);
            _logger.LogInformation($"END: GetBasketByInvestorIdAsync {investorId}");

            if (cachedData is null)
            {
                return null;
            }

            var result = _serializeService.Deserialize<Cart>(cachedData);

            return result;
        }

        public async Task<Cart> UpdateBasketAsync(Cart cart, DistributedCacheEntryOptions? options = null)
        {
            var serializedCart = _serializeService.Serialize(cart);

            if (options is not null)
            {
                await _redisCacheService.SetStringAsync(cart.InvestorId, serializedCart, options);
            }
            else
            {
                await _redisCacheService.SetStringAsync(cart.InvestorId, serializedCart);
            }
            
            var result = await this.GetBasketByInvestorIdAsync(cart.InvestorId);

            return result;
        }

        public async Task<bool> DeleteBasketByInvestorIdAsync(string investorId)
        {
            await _redisCacheService.RemoveAsync(investorId);
            return true;
        }
    }
}
