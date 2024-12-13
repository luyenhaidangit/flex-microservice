using AutoMapper;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Flex.Securities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuersController : ControllerBase
    {
        private readonly IIssuerRepository _repository;
        private readonly IMapper _mapper;

        public IssuersController(IIssuerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #region Query
        /// <summary>
        /// Lấy danh sách tất cả các Issuer.
        /// </summary>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllIssuersAsync()
        {
            var issuers = await _repository.GetAllIssuersAsync();
            var result = _mapper.Map<IEnumerable<IssuerDto>>(issuers);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin Issuer theo ID.
        /// </summary>
        [HttpGet("get-issuer-by-id/{id:long}")]
        public async Task<IActionResult> GetIssuerByIdAsync([FromRoute] long id)
        {
            var issuer = await _repository.GetIssuerByIdAsync(id);
            if (issuer == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<IssuerDto>(issuer);
            return Ok(result);
        }
        #endregion

        #region Command
        /// <summary>
        /// Tạo một Issuer mới.
        /// </summary>
        [HttpPost("create-issuer")]
        public async Task<IActionResult> CreateIssuerAsync([FromBody] CreateIssuerDto issuerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var issuer = _mapper.Map<CatalogIssuer>(issuerDto);

            await _repository.CreateIssuerAsync(issuer);

            var result = _mapper.Map<IssuerDto>(issuer);
            return CreatedAtAction(nameof(GetIssuerByIdAsync), new { id = issuer.Id }, result);
        }

        /// <summary>
        /// Cập nhật thông tin Issuer.
        /// </summary>
        [HttpPost("update-issuer")]
        public async Task<IActionResult> UpdateIssuerAsync([FromBody] UpdateIssuerDto issuerDto)
        {
            var issuerEntity = await _repository.GetIssuerByIdAsync(issuerDto.Id);
            if (issuerEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(issuerDto, issuerEntity);

            await _repository.UpdateIssuerAsync(issuerEntity);

            var result = _mapper.Map<IssuerDto>(issuerEntity);
            return Ok(result);
        }

        /// <summary>
        /// Xóa một Issuer theo ID.
        /// </summary>
        [HttpDelete("delete-issuer/{id:long}")]
        public async Task<IActionResult> DeleteIssuerAsync([FromRoute] long id)
        {
            var issuerEntity = await _repository.GetIssuerByIdAsync(id);
            if (issuerEntity == null)
            {
                return NotFound();
            }

            await _repository.DeleteIssuerAsync(id);

            return NoContent();
        }
        #endregion Command
    }
}
