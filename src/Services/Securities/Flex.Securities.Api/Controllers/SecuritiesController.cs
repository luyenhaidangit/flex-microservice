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
        [HttpPost("create-securities")]
        public async Task<IActionResult> CreateSecuritiesAsync([FromBody] CreateSecuritiesDto request)
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

        //#region Query
        ///// <summary>
        ///// Phân trang Yêu cầu Tổ chức phát hành.
        ///// </summary>
        //[HttpGet("get-request-paging")]
        //public async Task<IActionResult> GetRequestPagingIssuersAsync([FromQuery] GetIssuersPagingRequest request)
        //{
        //    var query = _issuerRequestRepository.FindAll().WhereIf(!string.IsNullOrEmpty(request.Name), b => b.Name.ToUpper().Contains(request.Name.ToUpper()));

        //    var resultPaged = await query.ToPagedResultAsync(request);

        //    var resultDtoPaged = resultPaged.MapPagedResult<CatalogIssuerRequest, IssuerPagedDto>(_mapper);

        //    return Ok(Result.Success(resultDtoPaged));
        //}

        ///// <summary>
        ///// Phân trang Tổ chức phát hành.
        ///// </summary>
        //[HttpGet("get-paging")]
        //public async Task<IActionResult> GetPagingIssuersAsync([FromQuery] GetIssuersPagingRequest request)
        //{
        //    var query = _issuerRepository.FindAll().WhereIf(!string.IsNullOrEmpty(request.Name), b => b.Name.ToUpper().Contains(request.Name.ToUpper()));

        //    var resultPaged = await query.ToPagedResultAsync(request);

        //    var resultDtoPaged = resultPaged.MapPagedResult<CatalogIssuer, IssuerPagedDto>(_mapper);

        //    return Ok(Result.Success(resultDtoPaged));
        //}

        ///// <summary>
        ///// Lấy thông tin Tổ chức phát hành theo Id.
        ///// </summary>
        //[HttpGet("get-issuer-by-id")]
        //public async Task<IActionResult> GetIssuerByIdAsync([FromQuery] EntityKey<long> entityKey)
        //{
        //    var isExistIssuer = await _issuerRepository.FindByCondition(x => x.Id == entityKey.Id).AnyAsync();
        //    if (!isExistIssuer)
        //    {
        //        return BadRequest(Result.Failure(message: "Issuer not found."));
        //    }

        //    var issuer = await _issuerRepository.FindByCondition(x => x.Id == entityKey.Id).Include(x => x.Securities).FirstAsync();

        //    var result = _mapper.Map<IssuerDto>(issuer);

        //    return Ok(Result.Success(result));
        //}
        //#endregion

        //#region Command
        ///// <summary>
        ///// Thêm mới Tổ chức phát hành.
        ///// </summary>
        //[HttpPost("create-issuer")]
        //public async Task<IActionResult> CreateIssuerAsync([FromBody] CreateIssuerDto issuerDto)
        //{
        //    var queryIssuer = _issuerRepository.FindAll();
        //    var queryIssuerRequest = _issuerRequestRepository.FindAll();

        //    // Validate
        //    // Check if issuer code is already exists in database
        //    var isCodeExistRequests = await queryIssuerRequest.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper() &&
        //        !queryIssuer.Any(i => i.Code.ToUpper() == x.Code.ToUpper())).AnyAsync();
        //    if (isCodeExistRequests)
        //    {
        //        return BadRequest(Result.Failure(message: "Issuer code is already exists in request list."));
        //    }

        //    var isCodeExistEntities = await queryIssuer.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper()).AnyAsync();
        //    if (isCodeExistEntities)
        //    {
        //        return BadRequest(Result.Failure(message: "Issuer code is already exists."));
        //    }

        //    // Process
        //    // Create issuer request
        //    var issuerRequest = _mapper.Map<CatalogIssuerRequest>(issuerDto);
        //    await _issuerRequestRepository.CreateAsync(issuerRequest);

        //    // Result

        //    return Ok(Result.Success());
        //}

        ///// <summary>
        ///// Cập nhật Tổ chức phát hành.
        ///// </summary>
        //[HttpPost("update-issuer")]
        //public async Task<IActionResult> UpdateIssuerAsync([FromBody] UpdateIssuerDto issuerDto)
        //{
        //    var queryIssuer = _issuerRepository.FindAll();
        //    var queryIssuerRequest = _issuerRequestRepository.FindAll();

        //    // Validate
        //    // Check if issuer id is already exists
        //    var issuer = await _issuerRepository.FindByCondition(x => x.Id == issuerDto.Id).FirstOrDefaultAsync();
        //    if (issuer == null)
        //    {
        //        return BadRequest(Result.Failure(message: "Issuer not found."));
        //    }

        //    if (issuer.Name.Equals(issuerDto.Name, StringComparison.OrdinalIgnoreCase) &&
        //        issuer.Code.Equals(issuerDto.Code, StringComparison.OrdinalIgnoreCase))
        //    {
        //        return BadRequest(Result.Failure(message: "No changes detected for Name or Code."));
        //    }

        //    // Check if issuer code is already exists in database
        //    var isCodeExistRequests = await queryIssuerRequest.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper() &&
        //        !queryIssuer.Any(i => i.Code.ToUpper() == x.Code.ToUpper() && x.EntityId != issuerDto.Id)).AnyAsync();
        //    if (isCodeExistRequests)
        //    {
        //        return BadRequest(Result.Failure(message: "Issuer code is already exists in request list."));
        //    }

        //    var isCodeExistEntities = await queryIssuer.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper() && x.Id != issuerDto.Id).AnyAsync();
        //    if (isCodeExistEntities)
        //    {
        //        return BadRequest(Result.Failure(message: "Issuer code is already exists."));
        //    }

        //    // Process
        //    // Begin: Transaction
        //    var transaction = _issuerRepository.BeginTransactionAsync();

        //    // Update issuer
        //    issuer.ProcessStatus = EProcessStatus.PendingUpdate;
        //    _issuerRepository.Update(issuer);

        //    // Create issuer request
        //    var issuerRequest = _mapper.Map<CatalogIssuerRequest>(issuerDto);
        //    issuerRequest.EntityId = issuerDto.Id;
        //    _issuerRequestRepository.Create(issuerRequest);

        //    await _issuerRepository.EndTransactionAsync();
        //    // End: Transaction

        //    return Ok(Result.Success());
        //}

        ///// <summary>
        ///// Xóa tổ chức phát hành.
        ///// </summary>
        //[HttpPost("delete-issuer")]
        //public async Task<IActionResult> DeleteIssuerAsync(EntityKey<long> entityKey)
        //{
        //    // Validate
        //    var isExistIssuer = await _issuerRepository.FindByCondition(x => x.Id == entityKey.Id).AnyAsync();
        //    if (!isExistIssuer)
        //    {
        //        return BadRequest(Result.Failure(message: "Issuer not found."));
        //    }

        //    // Process
        //    var issuer = await _issuerRepository.FindByCondition(x => x.Id == entityKey.Id).FirstAsync();
        //    issuer.ProcessStatus = EProcessStatus.PendingDelete;

        //    await _issuerRepository.UpdateAsync(issuer);

        //    return Ok(Result.Success());
        //}

        ///// <summary>
        ///// Duyệt Tổ chức phát hành.
        ///// </summary>
        //[HttpPost("approve-issuer")]
        //public async Task<IActionResult> ApproveIssuerAsync([FromBody] ApproveRequest<long> request)
        //{
        //    if (request.RequestTypeEnum == ERequestType.ADD)
        //    {
        //        // Validate
        //        var isExistRequests = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();

        //        if (!isExistRequests)
        //        {
        //            return BadRequest(Result.Failure(message: "Issuer request not found."));
        //        }

        //        var issuerRequest = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();
        //        if (issuerRequest.EntityId is not null)
        //        {
        //            return BadRequest(Result.Failure(message: "Issuer request is not pending create."));
        //        }

        //        // Begin: Transaction
        //        var transaction = _issuerRepository.BeginTransactionAsync();

        //        // Create issuers
        //        var issuer = _mapper.Map<CatalogIssuer>(issuerRequest);
        //        issuer.ProcessStatus = EProcessStatus.Complete;
        //        _issuerRepository.Create(issuer);

        //        // Delete add request
        //        _issuerRequestRepository.Delete(issuerRequest);

        //        await _issuerRepository.EndTransactionAsync();
        //        // End: Transaction
        //    }
        //    else if (request.RequestTypeEnum == ERequestType.EDIT)
        //    {
        //        // Validate
        //        var isExistRequests = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();

        //        if (!isExistRequests)
        //        {
        //            return BadRequest(Result.Failure(message: "Issuer request not found."));
        //        }

        //        // Begin: Transaction
        //        var transaction = _issuerRepository.BeginTransactionAsync();

        //        var issuerRequest = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();

        //        // Update issuers
        //        var issuer = _mapper.Map<CatalogIssuer>(issuerRequest);
        //        issuer.ProcessStatus = EProcessStatus.Complete;
        //        _issuerRepository.Update(issuer);

        //        // Delete add request
        //        _issuerRequestRepository.Delete(issuerRequest);

        //        await _issuerRepository.EndTransactionAsync();
        //        // End: Transaction
        //    } else if(request.RequestTypeEnum == ERequestType.DELETE)
        //    {
        //        // Validate
        //        var isExistEntity = await _issuerRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();
        //        if (!isExistEntity)
        //        {
        //            return BadRequest(Result.Failure(message: "Issuer not found."));
        //        }

        //        // Process
        //        var issuer = await _issuerRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();
        //        await _issuerRepository.DeleteAsync(issuer);
        //        // End: Transaction
        //    }

        //    // Result

        //    return Ok(Result.Success());
        //}
        //#endregion Command

        #region Query
        /// <summary>
        /// Lấy thông tin chứng khoán đã được phê duyệt theo ID.
        /// </summary>
        //[HttpGet("get-security-by-issuer/{issuerId:long}")]
        //public async Task<IActionResult> GetSecuritiesByIssuerAsync([FromRoute] long issuerId)
        //{
        //    var securities = await _repository.GetSecuritiesByIssuerAsync(issuerId);

        //    var result = _mapper.Map<IEnumerable<SecuritiesDto>>(securities);

        //    return Ok(result);
        //}

        /// <summary>
        /// Lấy thông tin chứng khoán theo Id.
        /// </summary>
        //[HttpGet("get-security-by-id/{securitiesId:long}")]
        //public async Task<IActionResult> GetSecuritiesByNoAsync([FromRoute] long securitiesId)
        //{
        //    var securities = await _repository.GetSecuritiesByIdAsync(securitiesId);

        //    var result = _mapper.Map<SecurityDto>(securities);

        //    return Ok(result);
        //}
        #endregion

        //#region Command
        ///// <summary>
        ///// Thêm mới chứng khoán.
        ///// </summary>
        //[HttpPost("create-security")]
        //public async Task<IActionResult> CreateSecuritiesAsync([FromBody] CreateSecuritiesDto securitiesDto)
        //{
        //    // Create
        //    var securities = _mapper.Map<CatalogSecurities>(securitiesDto);
        //    await _repository.CreateSecuritiesAsync(securities);

        //    // Result
        //    var result = _mapper.Map<SecurityDto>(securities);

        //    return Ok(result);
        //}

        ///// <summary>
        ///// Cập nhật chứng khoán.
        ///// </summary>
        //[HttpPost("update-security")]
        //public async Task<IActionResult> UpdateSecuritiesAsync([FromBody] UpdateSecuritiesDto securitiesDto)
        //{
        //    // Validate
        //    var securitiesEntity = await _repository.GetSecuritiesByIdAsync(securitiesDto.Id);
        //    if (securitiesEntity is null)
        //    {
        //        return NotFound();
        //    }

        //    // Update
        //    var updateSecurities = _mapper.Map(securitiesDto, securitiesEntity);
        //    await _repository.UpdateSecuritiesAsync(updateSecurities);

        //    // Result
        //    var result = _mapper.Map<SecurityDto>(updateSecurities);

        //    return Ok(result);
        //}

        ///// <summary>
        ///// Xóa chứng khoán.
        ///// </summary>
        //[HttpPost("delete-security/{securitiesId:long}")]
        //public async Task<IActionResult> DeleteSecuritiesAsync([FromRoute] long securitiesId)
        //{
        //    // Validate
        //    var securitiesEntity = await _repository.GetSecuritiesByIdAsync(securitiesId);
        //    if (securitiesEntity is null)
        //    {
        //        return NotFound();
        //    }

        //    // Result
        //    await _repository.DeleteSecuritiesAsync(securitiesId);
        //    return NoContent();
        //}
        //#endregion Command
    }
}
