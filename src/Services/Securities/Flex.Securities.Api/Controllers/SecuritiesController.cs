using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;
using Flex.Infrastructure.EF;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Flex.Securities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecuritiesController : ControllerBase
    {
        private readonly ISecuritiesRepository _securitiesRepository;
        private readonly IMapper _mapper;

        public SecuritiesController(ISecuritiesRepository securitiesRepository, IMapper mapper)
        {
            _securitiesRepository = securitiesRepository;
            _mapper = mapper;
        }

        #region Query
        /// <summary>
        /// Phân trang Danh sách chứng khoán.
        /// </summary>
        [HttpGet("get-securities-paging")]
        public async Task<IActionResult> GetPagingSecuritiesAsync([FromQuery] GetSecuritiesPagingRequest request)
        {
            var query = _securitiesRepository.FindAll().WhereIf(!string.IsNullOrEmpty(request.Symbol), b => b.Symbol.ToUpper().Contains(request.Symbol.ToUpper()));

            var resultPaged = await query.ToPagedResultAsync(request);

            var resultDtoPaged = resultPaged.MapPagedResult<CatalogSecurities, SecuritiesDto>(_mapper);

            return Ok(Result.Success(resultDtoPaged));
        }

        /// <summary>
        /// Thông tin Chi tiết chứng khoán.
        /// </summary>
        [HttpGet("get-security-by-id")]
        public async Task<IActionResult> GetSecuritiesByIdAsync([FromQuery] EntityKey<long> entityKey)
        {
            var securities = await _securitiesRepository.FindByCondition(x => x.Id == entityKey.Id).FirstOrDefaultAsync();
            if (securities is null)
            {
                return BadRequest(Result.Failure(message: "Securities not found."));
            }

            var result = _mapper.Map<SecuritiesDto>(securities);

            return Ok(Result.Success(result));
        }
        #endregion

        #region Command 
        /// <summary>
        /// Thêm mới Chứng khoán.
        /// </summary>
        [HttpPost("create-securities")]
        public async Task<IActionResult> CreateSecuritiesAsync([FromBody] CreateSecuritiesRequest request)
        {
            // Validate
            // Check if symbol code is already exists in database
            var isSymbolExist = await _securitiesRepository.FindByCondition(x => x.Symbol.ToUpper() == request.Symbol.ToUpper()).AnyAsync();
            if (isSymbolExist)
            {
                return BadRequest(Result.Failure(message: "Issuer symbol is already exists."));
            }

            // Process
            // Create issuer request
            var securities = _mapper.Map<CatalogSecurities>(request);
            await _securitiesRepository.CreateAsync(securities);

            // Result

            return Ok(Result.Success());
        }

        /// <summary>
        /// Cập nhật Chứng khoán.
        /// </summary>
        [HttpPost("update-securities")]
        public async Task<IActionResult> UpdateSecuritiesAsync([FromBody] UpdateSecuritiesRequest request)
        {
            var securities = await _securitiesRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (securities is null)
            {
                return BadRequest(Result.Failure(message: "Securities not found."));
            }

            // Validate
            // Check if symbol code is already exists in database
            if (!string.IsNullOrEmpty(request.Symbol) && !securities.Symbol.Equals(request.Symbol, StringComparison.OrdinalIgnoreCase))
            {
                var isSymbolExist = await _securitiesRepository.FindByCondition(x => x.Symbol.ToUpper() == request.Symbol.ToUpper() && x.Id != request.Id).AnyAsync();
                if (isSymbolExist)
                {
                    return Conflict(Result.Failure(message: "Securities symbol is already exists."));
                }
            }

            // Process
            // Update issuer request
            _mapper.Map(request, securities);

            await _securitiesRepository.UpdateAsync(securities);

            // Result

            return Ok(Result.Success());
        }

        /// <summary>
        /// Xoá Chứng khoán.
        /// </summary>
        [HttpPost("delete-securities")]
        public async Task<IActionResult> DeleteSecuritiesAsync([FromBody] EntityKey<long> request)
        {
            // Validate
            var securities = await _securitiesRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (securities is null)
            {
                return BadRequest(Result.Failure(message: "Securities not found."));
            }

            // Process
            // Delete securities request
            await _securitiesRepository.DeleteAsync(securities);

            // Result

            return Ok(Result.Success());
        }
        #endregion
    }
}
