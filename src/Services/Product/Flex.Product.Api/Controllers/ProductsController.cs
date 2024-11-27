using Flex.Product.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Flex.Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController
    {
        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository )
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<object> GetProducts()
        {
            var result = await _repository.GetProductsAsync();

            return result;
        }
    }
}
