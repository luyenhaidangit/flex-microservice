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
        /// Lấy thông tin Tổ chức phát hành theo Id.
        /// </summary>
        [HttpGet("get-issuer-by-id")]
        public async Task<IActionResult> GetIssuerByIdAsync([FromQuery] EntityKey<long> entityKey)
        {
            var issuer = await _repository.GetIssuerByIdAsync(entityKey.Key);

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
        /// Thêm mới Tổ chức phát hành
        /// </summary>
        [HttpPost("create-issuer")]
        public async Task<IActionResult> CreateIssuerAsync([FromBody] CreateIssuerDto issuerDto)
        {
            var issuer = _mapper.Map<CatalogIssuer>(issuerDto);

            // Process
            issuer.Status = EEntityStatus.Pending;

            await _repository.CreateIssuerAsync(issuer);

            // Result
            var result = _mapper.Map<IssuerDto>(issuer);

            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Duyệt Tổ chức phát hành
        /// </summary>
        [HttpPost("approve-issuer")]
        public async Task<IActionResult> ApproveIssuerAsync([FromBody] EntityKey<long> entityKey)
        {
            // Validate
            var issuer = await _repository.GetIssuerByIdAsync(entityKey.Key);

            if (issuer == null)
            {
                throw new ArgumentNullException("Issuer");
            }

            if (issuer.Status != EEntityStatus.Pending)
            {
                throw new InvalidOperationException("Only issuers in 'Pending' status can be approved.");
            }

            await _repository.ApproveIssuerAsync(entityKey.Key);

            // Result
            var result = _mapper.Map<IssuerDto>(issuer);

            return Ok(Result.Success(result));
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
