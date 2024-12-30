using Flex.Basket.Api.Entities;
using Flex.Basket.Api.Repositories.Interfaces;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Flex.Basket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly IBasketRepository _basketRepository;
        private readonly ILogger<BasketsController> _logger;

        public BasketsController(IBasketRepository basketRepository, ILogger<BasketsController> logger)
        {
            _basketRepository = basketRepository;
            _logger = logger;
        }

        /// <summary>
        /// Lấy giỏ lệnh theo nhà đầu tư.
        /// </summary>
        [HttpGet("get-basket-by-investor")]
        public async Task<IActionResult> GetBasketByInvestor([FromQuery] EntityKey<string> entityKey)
        {
            _logger.LogInformation($"BEGIN: GetBasketByInvestor {entityKey.Id}");
            var result = await _basketRepository.GetBasketByInvestorIdAsync(entityKey.Id);
            _logger.LogInformation($"END: GetBasketByInvestor {entityKey.Id}");

            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Cập nhật giỏ lệnh theo nhà đầu tư.
        /// </summary>
        [HttpPost("update-basket")]
        public async Task<IActionResult> UpdateBasket([FromBody] Cart cart)
        {
            _logger.LogInformation($"BEGIN: UpdateBasket for {cart.InvestorId}");
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddMinutes(10))
                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

            var result = await _basketRepository.UpdateBasketAsync(cart, options);
            _logger.LogInformation($"END: UpdateBasket for {cart.InvestorId}");

            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Xóa giỏ lệnh theo nhà đầu tư.
        /// </summary>
        [HttpPost("delete-basket")]
        public async Task<ActionResult<bool>> DeleteBasket([FromBody] EntityKey<string> entityKey)
        {
            _logger.LogInformation($"BEGIN: DeleteBasket {entityKey.Id}");
            var result = await _basketRepository.DeleteBasketByInvestorIdAsync(entityKey.Id);
            _logger.LogInformation($"END: DeleteBasket {entityKey.Id}");

            return Ok(Result.Success());
        }
    }
}
