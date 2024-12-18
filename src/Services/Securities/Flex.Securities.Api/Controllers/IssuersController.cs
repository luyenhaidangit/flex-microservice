using AutoMapper;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Flex.Infrastructure.EF;
using Flex.Shared.Enums.General;

namespace Flex.Securities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuersController : ControllerBase
    {
        private readonly IIssuerRepository _issuerRepository;
        private readonly IMapper _mapper;

        public IssuersController(IMapper mapper, IIssuerRepository repository, IIssuerRequestRepository issuerRequestRepository)
        {
            _mapper = mapper;
            _issuerRepository = repository;
        }

        #region Query
        /// <summary>
        /// Phân trang Tổ chức phát hành.
        /// </summary>
        [HttpGet("get-paging")]
        public async Task<IActionResult> GetPagingIssuersAsync([FromQuery] GetIssuersPagingRequest request)
        {
            var resultPaged = await _issuerRepository.GetPagingIssuersAsync(request);

            var resultDtoPaged = resultPaged.MapPagedResult<CatalogIssuer, IssuerPagedDto>(_mapper);
            
            return Ok(Result.Success(resultDtoPaged));
        }

        /// <summary>
        /// Lấy thông tin Tổ chức phát hành theo Id.
        /// </summary>
        [HttpGet("get-issuer-by-id")]
        public async Task<IActionResult> GetIssuerByIdAsync([FromQuery] EntityKey<long> entityKey)
        {
            var issuer = await _issuerRepository.GetIssuerByIdAsync(entityKey.Id);

            if (issuer is null)
            {
                return BadRequest(Result.Failure(message: "Issuer not found"));
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
            // Validate


            // Process
            var issuer = _mapper.Map<CatalogIssuer>(issuerDto);

            // Process
            issuer.Status = EEntityStatus.PENDING;

            await _issuerRepository.CreateIssuerAsync(issuer);

            // Result
            var result = _mapper.Map<IssuerPagedDto>(issuer);

            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Duyệt Tổ chức phát hành
        /// </summary>
        [HttpPost("approve-issuer")]
        public async Task<IActionResult> ApproveIssuerAsync([FromBody] EntityKey<long> entityKey)
        {
            // Validate
            var issuer = await _issuerRepository.GetIssuerByIdAsync(entityKey.Id);

            if (issuer is null)
            {
                return BadRequest(Result.Failure(message: "Issuer not found"));
            }

            //if (issuer.Status != EEntityStatus.Pending)
            //{
            //    return BadRequest(Result.Failure(message: "Only issuers in 'Pending' status can be approved."));
            //}

            await _issuerRepository.ApproveIssuerAsync(issuer);

            // Result
            var result = _mapper.Map<IssuerPagedDto>(issuer);

            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Cập nhật Tổ chức phát hành.
        /// </summary>
        [HttpPost("update-issuer")]
        public async Task<IActionResult> UpdateIssuerAsync([FromBody] UpdateIssuerDto issuerDto)
        {
            // Validate
            var issuer = await _issuerRepository.GetIssuerByIdAsync(issuerDto.Id);

            if (issuer is null)
            {
                return BadRequest(Result.Failure(message: "Issuer not found"));
            }

            //if (issuer.Status is EEntityStatus.Pending)
            //{
            //    return BadRequest(Result.Failure(message: "Only issuers in 'Pending' status can be approved."));
            //}

            _mapper.Map(issuerDto, issuer);

            await _issuerRepository.UpdateIssuerAsync(issuer);

            return Ok(Result.Success(issuer));
        }

        /// <summary>
        /// Xóa một Issuer theo ID.
        /// </summary>
        [HttpPost("delete-issuer/{id:long}")]
        public async Task<IActionResult> DeleteIssuerAsync([FromRoute] long id)
        {
            var issuerEntity = await _issuerRepository.GetIssuerByIdAsync(id);
            if (issuerEntity == null)
            {
                return NotFound();
            }

            await _issuerRepository.DeleteIssuerAsync(id);

            return NoContent();
        }
        #endregion Command
    }
}
