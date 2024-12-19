using AutoMapper;
using Newtonsoft.Json;
using Flex.Securities.Api.Entities;
using Flex.Securities.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.Securities;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Flex.Infrastructure.EF;
using Flex.Shared.Constants;
using Flex.Shared.Enums.General;
using Microsoft.EntityFrameworkCore;
using Flex.Shared.SeedWork.General;
using Flex.Contracts.Domains.Interfaces;
using Flex.Securities.Api.Persistence;

namespace Flex.Securities.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssuersController : ControllerBase
    {
        private readonly IIssuerRepository _issuerRepository;
        private readonly IIssuerRequestRepository _issuerRequestRepository;
        private readonly IUnitOfWork<SecuritiesDbContext> _unitOfWork;
        private readonly IMapper _mapper;

        public IssuersController(IMapper mapper,
            IUnitOfWork<SecuritiesDbContext> unitOfWork,
            IIssuerRepository repository, 
            IIssuerRequestRepository issuerRequestRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _issuerRepository = repository;
            _issuerRequestRepository = issuerRequestRepository;
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
            var queryIssuer = _issuerRepository.FindAll();
            var queryIssuerRequest = _issuerRequestRepository.FindAll();

            // Validate
            // Check if issuer code is already exists in database
            var isCodeExistRequests = await queryIssuerRequest.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper() &&
                !queryIssuer.Any(i => i.Code.ToUpper() == x.Code.ToUpper())).AnyAsync();
            if (isCodeExistRequests)
            {
                return BadRequest(Result.Failure(message: "Issuer code is already exists in request list."));
            }

            var isNameExistRequests = await queryIssuerRequest.Where(x => x.Name.ToUpper() == issuerDto.Name.ToUpper() &&
                !queryIssuer.Any(i => i.Name.ToUpper() == x.Name.ToUpper())).AnyAsync();
            if (isNameExistRequests)
            {
                return BadRequest(Result.Failure(message: "Issuer name is already exists in request list."));
            }

            // Check if issuer name is already exists in database
            var isCodeExistEntities = await queryIssuer.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper()).AnyAsync();
            if (isCodeExistEntities)
            {
                return BadRequest(Result.Failure(message: "Issuer code is already exists."));
            }

            var isNameExistEntities = await queryIssuer.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper()).AnyAsync();
            if (isCodeExistEntities)
            {
                return BadRequest(Result.Failure(message: "Issuer name is already exists."));
            }

            var issuerRequest = _mapper.Map<CatalogIssuerRequest>(issuerDto);

            // Process
            // Create issuer request
            await _issuerRequestRepository.CreateAsync(issuerRequest);

            // Result

            return Ok(Result.Success());
        }

        /// <summary>
        /// Duyệt Tổ chức phát hành
        /// </summary>
        [HttpPost("approve-issuer")]
        public async Task<IActionResult> ApproveIssuerAsync([FromBody] ApproveRequest<long> request)
        {
            if (request.RequestTypeEnum == ERequestType.ADD)
            {
                // Validate
                var isExistRequests = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();

                if(!isExistRequests)
                {
                    return BadRequest(Result.Failure(message: "Issuer request not found"));
                }

                // Process with transaction
                var issuerRequest = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();
                var transaction = _issuerRequestRepository.BeginTransactionAsync();

                // Delete add request
                _issuerRequestRepository.Delete(issuerRequest);

                // Create issuers
                var issuer = _mapper.Map<CatalogIssuer>(issuerRequest);
                issuer.ProcessStatus = EProcessStatus.Complete;
                _issuerRepository.Create(issuer);

                await _issuerRequestRepository.EndTransactionAsync();
            }

            // Result

            return Ok(Result.Success());
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
