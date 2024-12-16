using AutoMapper;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;
using Flex.Shared.Enums;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Flex.Infrastructure.EF;

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
        /// Phân trang Tổ chức phát hành.
        /// </summary>
        [HttpGet("get-paging")]
        public async Task<IActionResult> GetPagingIssuersAsync([FromQuery] GetIssuersPagingRequest request)
        {
            var resultPaged = await _repository.GetPagingIssuersAsync(request);

            var resultDtoPaged = resultPaged.MapPagedResult<CatalogIssuer, IssuerPagedDto>(_mapper);
            
            return Ok(Result.Success(resultDtoPaged));
        }

        /// <summary>
        /// Thông tin Tổ chức phát hành theo Id.
        /// </summary>
        [HttpGet("get-issuer-by-id/{id:long}")]
        public async Task<IActionResult> GetIssuerByIdAsync([FromRoute] long id)
        {
            var issuer = await _repository.GetIssuerByIdAsync(id);

            if (issuer is null)
            {
                return NotFound(Result.Failure(message: "Issuer not found"));
            }

            var result = _mapper.Map<IssuerDto>(issuer);

            return Ok(Result.Success(result));
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

            // Process
            issuer.Status = EEntityStatus.Pending;

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
        [HttpPost("delete-issuer/{id:long}")]
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
