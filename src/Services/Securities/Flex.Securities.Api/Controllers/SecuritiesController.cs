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
        [HttpPost("create-securities")]
        public async Task<IActionResult> CreateSecuritiesAsync([FromBody] CreateSecuritiesDto securitiesDto)
        {
            // Validate
            var securitiesEntity = await _repository.GetSecuritiesByNoAsync(securitiesDto.No);
            if (securitiesEntity is not null)
            {
                return BadRequest($"Product No: {securitiesDto.No} is existed.");
            }

            // Create
            var securities = _mapper.Map<CatalogSecurities>(securitiesDto);
            await _repository.CreateSecuritiesAsync(securities);

            // Result
            var result = _mapper.Map<SecuritiesDto>(securities);

            return Ok(result);
        }

        [HttpPut("update-securities")]
        public async Task<IActionResult> UpdateSecuritiesAsync([FromBody] UpdateSecuritiesDto securitiesDto)
        {
            // Validate
            var securitiesEntity = await _repository.GetSecuritiesByNoAsync(securitiesDto.No);
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

        [HttpPut("delete-securities")]
        public async Task<IActionResult> DeleteSecuritiesAsync([Required] string securitiesNo)
        {
            // Validate
            var securitiesEntity = await _repository.GetSecuritiesByNoAsync(securitiesNo);
            if (securitiesEntity is null)
            {
                return NotFound();
            }

            // Result
            await _repository.DeleteSecuritiesAsync(securitiesNo);
            return NoContent();
        }
        #endregion Command
    }
}
