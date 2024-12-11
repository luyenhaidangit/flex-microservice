using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
        [HttpGet("get-securities-by-issuer/{issuerId:long}")]
        public async Task<IActionResult> GetSecuritiesByIssuerAsync([FromRoute] long issuerId)
        {
            var securities = await _repository.GetSecuritiesByIssuerAsync(issuerId);

            var result = _mapper.Map<IEnumerable<SecuritiesDto>>(securities);

            return Ok(result);
        }

        [HttpGet("get-securities-by-id/{securitiesId:long}")]
        public async Task<IActionResult> GetSecuritiesByNoAsync([FromRoute] long securitiesId)
        {
            var securities = await _repository.GetSecuritiesByIdAsync(securitiesId);

            var result = _mapper.Map<SecuritiesDto>(securities);

            return Ok(result);
        }
        #endregion

        #region Command
        [HttpPost("create-securities")]
        public async Task<IActionResult> CreateSecuritiesAsync([FromBody] CreateSecuritiesDto securitiesDto)
        {
            // Create
            var securities = _mapper.Map<CatalogSecurities>(securitiesDto);
            await _repository.CreateSecuritiesAsync(securities);

            // Result
            var result = _mapper.Map<SecuritiesDto>(securities);

            return Ok(result);
        }

        [HttpPost("update-securities")]
        public async Task<IActionResult> UpdateSecuritiesAsync([FromBody] UpdateSecuritiesDto securitiesDto)
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
            var result = _mapper.Map<SecuritiesDto>(updateSecurities);

            return Ok(result);
        }

        [HttpPost("delete-securities/{securitiesId:long}")]
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
