using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;

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

        #region Query
        /// <summary>
        /// Lấy thông tin chứng khoán đã được phê duyệt theo ID.
        /// </summary>
        [HttpGet("get-security-by-issuer/{issuerId:long}")]
        public async Task<IActionResult> GetSecuritiesByIssuerAsync([FromRoute] long issuerId)
        {
            var securities = await _repository.GetSecuritiesByIssuerAsync(issuerId);

            var result = _mapper.Map<IEnumerable<SecurityDto>>(securities);

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin chứng khoán theo Id.
        /// </summary>
        [HttpGet("get-security-by-id/{securitiesId:long}")]
        public async Task<IActionResult> GetSecuritiesByNoAsync([FromRoute] long securitiesId)
        {
            var securities = await _repository.GetSecuritiesByIdAsync(securitiesId);

            var result = _mapper.Map<SecurityDto>(securities);

            return Ok(result);
        }
        #endregion

        #region Command
        /// <summary>
        /// Thêm mới chứng khoán.
        /// </summary>
        [HttpPost("create-security")]
        public async Task<IActionResult> CreateSecuritiesAsync([FromBody] CreateSecuritiesDto securitiesDto)
        {
            // Create
            var securities = _mapper.Map<CatalogSecurity>(securitiesDto);
            await _repository.CreateSecuritiesAsync(securities);

            // Result
            var result = _mapper.Map<SecurityDto>(securities);

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật chứng khoán.
        /// </summary>
        [HttpPost("update-security")]
        public async Task<IActionResult> UpdateSecuritiesAsync([FromBody] UpdateSecurityDto securitiesDto)
        {
            // Validate
            var securitiesEntity = await _repository.GetSecuritiesByIdAsync(securitiesDto.Id);
            if (securitiesEntity is null)
            {
                return NotFound();
            }

            // Update
            var updateSecurities = _mapper.Map(securitiesDto, securitiesEntity);
            await _repository.UpdateSecuritiesAsync(updateSecurities);

            // Result
            var result = _mapper.Map<SecurityDto>(updateSecurities);

            return Ok(result);
        }

        /// <summary>
        /// Xóa chứng khoán.
        /// </summary>
        [HttpPost("delete-security/{securitiesId:long}")]
        public async Task<IActionResult> DeleteSecuritiesAsync([FromRoute] long securitiesId)
        {
            // Validate
            var securitiesEntity = await _repository.GetSecuritiesByIdAsync(securitiesId);
            if (securitiesEntity is null)
            {
                return NotFound();
            }

            // Result
            await _repository.DeleteSecuritiesAsync(securitiesId);
            return NoContent();
        }
        #endregion Command
    }
}
