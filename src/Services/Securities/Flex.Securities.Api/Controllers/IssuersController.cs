//using AutoMapper;
//using Flex.Securities.Api.Entities;
//using Flex.Securities.Api.Repositories.Interfaces;
//using Flex.Shared.DTOs.Securities;
//using Flex.Shared.SeedWork;
//using Microsoft.AspNetCore.Mvc;
//using Flex.Infrastructure.EF;
//using Flex.Shared.Enums.General;
//using Microsoft.EntityFrameworkCore;
//using Flex.Shared.SeedWork.General;

//namespace Flex.Securities.Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class IssuersController : ControllerBase
//    {
//        //private readonly IIssuerRepository _issuerRepository;
//        //private readonly IIssuerRequestRepository _issuerRequestRepository;
//        //private readonly IMapper _mapper;

//        //public IssuersController(IMapper mapper,IIssuerRepository repository, IIssuerRequestRepository issuerRequestRepository)
//        //{
//        //    _mapper = mapper;
//        //    _issuerRepository = repository;
//        //    _issuerRequestRepository = issuerRequestRepository;
//        //}

//        //#region Query
//        ///// <summary>
//        ///// Phân trang Yêu cầu Tổ chức phát hành.
//        ///// </summary>
//        //[HttpGet("get-request-paging")]
//        //public async Task<IActionResult> GetRequestPagingIssuersAsync([FromQuery] GetIssuersPagingRequest request)
//        //{
//        //    var query = _issuerRequestRepository.FindAll().WhereIf(!string.IsNullOrEmpty(request.Name), b => b.Name.ToUpper().Contains(request.Name.ToUpper()));

//        //    var resultPaged = await query.ToPagedResultAsync(request);

//        //    var resultDtoPaged = resultPaged.MapPagedResult<CatalogIssuerRequest, IssuerPagedDto>(_mapper);

//        //    return Ok(Result.Success(resultDtoPaged));
//        //}

//        ///// <summary>
//        ///// Phân trang Tổ chức phát hành.
//        ///// </summary>
//        //[HttpGet("get-paging")]
//        //public async Task<IActionResult> GetPagingIssuersAsync([FromQuery] GetIssuersPagingRequest request)
//        //{
//        //    var query = _issuerRepository.FindAll().WhereIf(!string.IsNullOrEmpty(request.Name), b => b.Name.ToUpper().Contains(request.Name.ToUpper()));

//        //    var resultPaged = await query.ToPagedResultAsync(request);

//        //    var resultDtoPaged = resultPaged.MapPagedResult<CatalogIssuer, IssuerPagedDto>(_mapper);
            
//        //    return Ok(Result.Success(resultDtoPaged));
//        //}

//        ///// <summary>
//        ///// Lấy thông tin Tổ chức phát hành theo Id.
//        ///// </summary>
//        //[HttpGet("get-issuer-by-id")]
//        //public async Task<IActionResult> GetIssuerByIdAsync([FromQuery] EntityKey<long> entityKey)
//        //{
//        //    var isExistIssuer = await _issuerRepository.FindByCondition(x => x.Id == entityKey.Id).AnyAsync();
//        //    if (!isExistIssuer)
//        //    {
//        //        return BadRequest(Result.Failure(message: "Issuer not found."));
//        //    }

//        //    var issuer = await _issuerRepository.FindByCondition(x => x.Id == entityKey.Id).Include(x => x.Securities).FirstAsync();

//        //    var result = _mapper.Map<IssuerDto>(issuer);

//        //    return Ok(Result.Success(result));
//        //}
//        //#endregion

//        //#region Command
//        ///// <summary>
//        ///// Thêm mới Tổ chức phát hành.
//        ///// </summary>
//        //[HttpPost("create-issuer")]
//        //public async Task<IActionResult> CreateIssuerAsync([FromBody] CreateIssuerDto issuerDto)
//        //{
//        //    var queryIssuer = _issuerRepository.FindAll();
//        //    var queryIssuerRequest = _issuerRequestRepository.FindAll();

//        //    // Validate
//        //    // Check if issuer code is already exists in database
//        //    var isCodeExistRequests = await queryIssuerRequest.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper() &&
//        //        !queryIssuer.Any(i => i.Code.ToUpper() == x.Code.ToUpper())).AnyAsync();
//        //    if (isCodeExistRequests)
//        //    {
//        //        return BadRequest(Result.Failure(message: "Issuer code is already exists in request list."));
//        //    }

//        //    var isCodeExistEntities = await queryIssuer.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper()).AnyAsync();
//        //    if (isCodeExistEntities)
//        //    {
//        //        return BadRequest(Result.Failure(message: "Issuer code is already exists."));
//        //    }

//        //    // Process
//        //    // Create issuer request
//        //    var issuerRequest = _mapper.Map<CatalogIssuerRequest>(issuerDto);
//        //    await _issuerRequestRepository.CreateAsync(issuerRequest);

//        //    // Result

//        //    return Ok(Result.Success());
//        //}

//        ///// <summary>
//        ///// Cập nhật Tổ chức phát hành.
//        ///// </summary>
//        //[HttpPost("update-issuer")]
//        //public async Task<IActionResult> UpdateIssuerAsync([FromBody] UpdateIssuerDto issuerDto)
//        //{
//        //    var queryIssuer = _issuerRepository.FindAll();
//        //    var queryIssuerRequest = _issuerRequestRepository.FindAll();

//        //    // Validate
//        //    // Check if issuer id is already exists
//        //    var issuer = await _issuerRepository.FindByCondition(x => x.Id == issuerDto.Id).FirstOrDefaultAsync();
//        //    if (issuer == null)
//        //    {
//        //        return BadRequest(Result.Failure(message: "Issuer not found."));
//        //    }

//        //    if (issuer.Name.Equals(issuerDto.Name, StringComparison.OrdinalIgnoreCase) &&
//        //        issuer.Code.Equals(issuerDto.Code, StringComparison.OrdinalIgnoreCase))
//        //    {
//        //        return BadRequest(Result.Failure(message: "No changes detected for Name or Code."));
//        //    }

//        //    // Check if issuer code is already exists in database
//        //    var isCodeExistRequests = await queryIssuerRequest.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper() &&
//        //        !queryIssuer.Any(i => i.Code.ToUpper() == x.Code.ToUpper() && x.EntityId != issuerDto.Id)).AnyAsync();
//        //    if (isCodeExistRequests)
//        //    {
//        //        return BadRequest(Result.Failure(message: "Issuer code is already exists in request list."));
//        //    }

//        //    var isCodeExistEntities = await queryIssuer.Where(x => x.Code.ToUpper() == issuerDto.Code.ToUpper() && x.Id != issuerDto.Id).AnyAsync();
//        //    if (isCodeExistEntities)
//        //    {
//        //        return BadRequest(Result.Failure(message: "Issuer code is already exists."));
//        //    }

//        //    // Process
//        //    // Begin: Transaction
//        //    var transaction = _issuerRepository.BeginTransactionAsync();

//        //    // Update issuer
//        //    issuer.ProcessStatus = EProcessStatus.PendingUpdate;
//        //    _issuerRepository.Update(issuer);

//        //    // Create issuer request
//        //    var issuerRequest = _mapper.Map<CatalogIssuerRequest>(issuerDto);
//        //    issuerRequest.EntityId = issuerDto.Id;
//        //    _issuerRequestRepository.Create(issuerRequest);

//        //    await _issuerRepository.EndTransactionAsync();
//        //    // End: Transaction

//        //    return Ok(Result.Success());
//        //}

//        ///// <summary>
//        ///// Xóa tổ chức phát hành.
//        ///// </summary>
//        //[HttpPost("delete-issuer")]
//        //public async Task<IActionResult> DeleteIssuerAsync(EntityKey<long> entityKey)
//        //{
//        //    // Validate
//        //    var isExistIssuer = await _issuerRepository.FindByCondition(x => x.Id == entityKey.Id).AnyAsync();
//        //    if (!isExistIssuer)
//        //    {
//        //        return BadRequest(Result.Failure(message: "Issuer not found."));
//        //    }

//        //    // Process
//        //    var issuer = await _issuerRepository.FindByCondition(x => x.Id == entityKey.Id).FirstAsync();
//        //    issuer.ProcessStatus = EProcessStatus.PendingDelete;

//        //    await _issuerRepository.UpdateAsync(issuer);

//        //    return Ok(Result.Success());
//        //}

//        ///// <summary>
//        ///// Duyệt Tổ chức phát hành.
//        ///// </summary>
//        //[HttpPost("approve-issuer")]
//        //public async Task<IActionResult> ApproveIssuerAsync([FromBody] ApproveRequest<long> request)
//        //{
//        //    if (request.RequestTypeEnum == ERequestType.ADD)
//        //    {
//        //        // Validate
//        //        var isExistRequests = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();

//        //        if (!isExistRequests)
//        //        {
//        //            return BadRequest(Result.Failure(message: "Issuer request not found."));
//        //        }

//        //        var issuerRequest = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();
//        //        if (issuerRequest.EntityId is not null)
//        //        {
//        //            return BadRequest(Result.Failure(message: "Issuer request is not pending create."));
//        //        }

//        //        // Begin: Transaction
//        //        var transaction = _issuerRepository.BeginTransactionAsync();

//        //        // Create issuers
//        //        var issuer = _mapper.Map<CatalogIssuer>(issuerRequest);
//        //        issuer.ProcessStatus = EProcessStatus.Complete;
//        //        _issuerRepository.Create(issuer);

//        //        // Delete add request
//        //        _issuerRequestRepository.Delete(issuerRequest);

//        //        await _issuerRepository.EndTransactionAsync();
//        //        // End: Transaction
//        //    }
//        //    else if (request.RequestTypeEnum == ERequestType.EDIT)
//        //    {
//        //        // Validate
//        //        var isExistRequests = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();

//        //        if (!isExistRequests)
//        //        {
//        //            return BadRequest(Result.Failure(message: "Issuer request not found."));
//        //        }

//        //        // Begin: Transaction
//        //        var transaction = _issuerRepository.BeginTransactionAsync();

//        //        var issuerRequest = await _issuerRequestRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();

//        //        // Update issuers
//        //        var issuer = _mapper.Map<CatalogIssuer>(issuerRequest);
//        //        issuer.ProcessStatus = EProcessStatus.Complete;
//        //        _issuerRepository.Update(issuer);

//        //        // Delete add request
//        //        _issuerRequestRepository.Delete(issuerRequest);

//        //        await _issuerRepository.EndTransactionAsync();
//        //        // End: Transaction
//        //    } else if(request.RequestTypeEnum == ERequestType.DELETE)
//        //    {
//        //        // Validate
//        //        var isExistEntity = await _issuerRepository.FindByCondition(x => x.Id == request.Id).AnyAsync();
//        //        if (!isExistEntity)
//        //        {
//        //            return BadRequest(Result.Failure(message: "Issuer not found."));
//        //        }

//        //        // Process
//        //        var issuer = await _issuerRepository.FindByCondition(x => x.Id == request.Id).FirstAsync();
//        //        await _issuerRepository.DeleteAsync(issuer);
//        //        // End: Transaction
//        //    }

//        //    // Result

//        //    return Ok(Result.Success());
//        //}
//        //#endregion Command
//    }
//}
