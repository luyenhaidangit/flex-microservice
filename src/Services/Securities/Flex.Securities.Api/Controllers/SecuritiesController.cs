using AutoMapper;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Flex.Securities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecuritiesController : ControllerBase
    {
        private readonly ISecuritiesRepository _repository;
        private readonly IMapper _mapper;

        public SecuritiesController(ISecuritiesRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        //// Query
        //Task<List<CatalogSecurities>> GetSecuritiesByIssuerAsync(string issuerNo);
        //Task<CatalogSecurities?> GetSecuritiesByNoAsync(string securitiesNo);
        //Task<string> GenerateSecuritiesNo();

        //// Command
        //Task CreateSecuritiesAsync(CatalogSecurities securities);
        //Task UpdateSecuritiesAsync(CatalogSecurities securities);
        //Task DeleteSecuritiesAsync(string securitiesNo);

        #region Query
        [HttpGet("get-securities-by-issuer/{issuerNo}")]
        public async Task<IActionResult> GetSecuritiesByIssuerAsync([Required] string issuerNo)
        {
            var securities = await _repository.GetSecuritiesByIssuerAsync(issuerNo);

            var result = _mapper.Map<IEnumerable<SecuritiesDto>>(securities);

            return Ok(result);
        }

        [HttpGet("get-securities-by-no/{securitiesNo}")]
        public async Task<IActionResult> GetSecuritiesByNoAsync([Required] string securitiesNo)
        {
            var securities = await _repository.GetSecuritiesByNoAsync(securitiesNo);

            var result = _mapper.Map<SecuritiesDto>(securities);

            return Ok(result);
        }
        #endregion

        #region Command
        #endregion

        //#region CRUD
        //[HttpGet]
        //[ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
        //public async Task<IActionResult> GetProducts()
        //{
        //    var products = await _repository.GetProductsAsync();
        //    var result = _mapper.Map<IEnumerable<ProductDto>>(products);
        //    return Ok(result);
        //}

        //[HttpGet("{id:long}")]
        //[ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
        //public async Task<IActionResult> GetProduct([Required] long id)
        //{
        //    var product = await _repository.GetProductAsync(id);
        //    if (product == null) return NotFound();

        //    var result = _mapper.Map<ProductDto>(product);
        //    return Ok(result);
        //}

        //[HttpPost]
        //[ClaimRequirement(FunctionCode.PRODUCT, CommandCode.CREATE)]
        //public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        //{
        //    var productEntity = await _repository.GetProductByNoAsync(productDto.No);
        //    if (productEntity != null) return BadRequest($"Product No: {productDto.No} is existed.");

        //    var product = _mapper.Map<CatalogProduct>(productDto);
        //    await _repository.CreateProductAsync(product);
        //    var result = _mapper.Map<ProductDto>(product);
        //    return Ok(result);
        //}

        //[HttpPut("{id:long}")]
        //[ClaimRequirement(FunctionCode.PRODUCT, CommandCode.UPDATE)]
        //public async Task<IActionResult> UpdateProduct([Required] long id, [FromBody] UpdateProductDto productDto)
        //{
        //    var product = await _repository.GetProductAsync(id);
        //    if (product == null) return NotFound();

        //    var updateProduct = _mapper.Map(productDto, product);
        //    await _repository.UpdateProductAsync(updateProduct);
        //    var result = _mapper.Map<ProductDto>(product);
        //    return Ok(result);
        //}

        //[HttpDelete("{id:long}")]
        //[ClaimRequirement(FunctionCode.PRODUCT, CommandCode.DELETE)]
        //public async Task<IActionResult> DeleteProduct([Required] long id)
        //{
        //    var product = await _repository.GetProductAsync(id);
        //    if (product == null) return NotFound();

        //    await _repository.DeleteProductAsync(id);
        //    return NoContent();
        //}

        //#endregion

        //#region Additional Resources

        //[HttpGet("get-product-by-no/{productNo}")]
        //[ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
        //public async Task<IActionResult> GetProductByNo([Required] string productNo)
        //{
        //    var product = await _repository.GetProductByNoAsync(productNo);
        //    if (product == null) return NotFound();

        //    var result = _mapper.Map<ProductDto>(product);
        //    return Ok(result);
        //}

        //#endregion
    }
}
