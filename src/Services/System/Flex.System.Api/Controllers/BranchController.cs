using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.System.Branch;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Flex.Shared.Constants.Common;

namespace Flex.System.Api.Controllers
{
    /// <summary>
    /// Quản lý vòng đời Chi nhánh: 
    ///     • Truy vấn (paging / detail / history)
    ///     • Gửi yêu cầu (tạo / cập nhật / xóa)
    ///     • Phê duyệt – Từ chối – Huỷ
    ///     • Lưu vết (BranchHistory)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchRequestRepository _branchRequestRepository;
        private readonly IBranchHistoryRepository _branchHistoryRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BranchController(
            IBranchRepository branchRepository,
            IBranchRequestRepository branchRequestRepository,
            IBranchHistoryRepository branchHistoryRepository,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _branchRepository = branchRepository;
            _branchRequestRepository = branchRequestRepository;
            _branchHistoryRepository = branchHistoryRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        #region Query

        [HttpGet("get-branches-paging")]
        public async Task<IActionResult> GetPagingBranchesAsync([FromQuery] GetBranchesPagingRequest request)
        {
            var branchesQuery = _branchRepository.FindAll();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                branchesQuery = branchesQuery.Where(x => x.Name.Contains(request.Keyword) || x.Code.Contains(request.Keyword));
            }
            if (!string.IsNullOrEmpty(request.Status))
            {
                branchesQuery = branchesQuery.Where(x => x.Status == request.Status);
            }

            var branches = await branchesQuery.ToListAsync();
            var branchDict = branches.ToDictionary(x => x.Id);

            var requestQuery = _branchRequestRepository.FindByCondition(x => x.Status == StatusConstant.Pending);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                requestQuery = requestQuery.Where(x => x.ProposedData.Contains(request.Keyword));
            }

            var pendingRequests = await requestQuery.ToListAsync();

            var resultList = branches
                .GroupJoin(
                    pendingRequests.Where(r => r.BranchId != null),
                    branch => branch.Id,
                    request => request.BranchId!.Value,
                    (branch, requests) =>
                    {
                        var pending = requests.FirstOrDefault();
                        return new BranchPagingDto
                        {
                            Id = branch.Id,
                            Code = branch.Code,
                            Name = branch.Name,
                            Address = branch.Address,
                            Region = branch.Region,
                            Status = branch.Status,
                            HasPendingRequest = pending != null,
                            PendingRequestType = pending?.RequestType,
                            RequestedBy = pending?.RequestedBy,
                            RequestedDate = pending?.CreatedDate
                        };
                    })
                .ToList();

            var createRequests = pendingRequests
                .Where(r => r.BranchId == null || !branchDict.ContainsKey(r.BranchId.Value))
                .Select(r => new BranchPagingDto
                {
                    Id = 0,
                    Code = "---",
                    Name = "(Đề xuất tạo mới)",
                    Address = "---",
                    Region = "---",
                    Status = StatusConstant.Pending,
                    HasPendingRequest = true,
                    PendingRequestType = r.RequestType,
                    RequestedBy = r.RequestedBy,
                    RequestedDate = r.CreatedDate
                });

            resultList.AddRange(createRequests);

            var sortedResult = resultList
                .OrderBy(x => x.Status != StatusConstant.Pending)
                .ThenByDescending(x => x.RequestedDate ?? DateTimeOffset.MinValue)
                .ToList();

            int pageIndex = request.PageIndex ?? 1;
            int pageSize = request.PageSize ?? sortedResult.Count;

            var pagedResult = sortedResult
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new PagedResult<BranchPagingDto>
            {
                Items = pagedResult,
                TotalItems = sortedResult.Count,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return Ok(Result.Success(response));
        }

        [HttpGet("get-branch-by-id")]
        public async Task<IActionResult> GetByIdAsync([FromQuery] long id)
        {
            var branch = await _branchRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (branch == null)
                return NotFound(Result.Failure(message: "Branch not found."));

            var dto = _mapper.Map<BranchDto>(branch);
            return Ok(Result.Success(dto));
        }

        [HttpGet("get-branch-history")]
        public async Task<IActionResult> GetHistoryAsync([FromQuery] long branchId)
        {
            var histories = await _branchHistoryRepository.FindByCondition(x => x.BranchId == branchId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            var result = _mapper.Map<List<BranchHistoryDto>>(histories);
            return Ok(Result.Success(result));
        }

        #endregion

        #region Command

        [HttpPost("create-branch-request")]
        public async Task<IActionResult> CreateBranchRequestAsync([FromBody] CreateBranchRequest request)
        {
            var hasPending = await _branchRequestRepository
                .FindByCondition(x =>
                    x.Status == StatusConstant.Pending &&
                    x.RequestType == RequestTypeConstant.Create &&
                    x.ProposedData.Contains($"\"Code\":\"{request.Code}\""))
                .AnyAsync();

            if (hasPending)
                return BadRequest(Result.Failure("Đã có yêu cầu tạo chi nhánh với mã này đang chờ phê duyệt."));

            var isExist = await _branchRepository.FindByCondition(x => x.Code == request.Code).AnyAsync();
            if (isExist)
                return BadRequest(Result.Failure("Mã chi nhánh đã tồn tại."));

            var createRequest = request.ToCreateBranchRequest();
            await _branchRequestRepository.CreateAsync(createRequest);

            return Ok(Result.Success());
        }

        [HttpPost("update-branch-request")]
        public async Task<IActionResult> UpdateBranchRequestAsync([FromBody] UpdateBranchRequest request)
        {
            var entity = await _branchRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (entity == null)
                return NotFound(Result.Failure("Branch not found."));

            var updateRequest = new BranchRequest
            {
                BranchId = request.Id,
                RequestType = RequestTypeConstant.Update,
                ProposedData = JsonSerializer.Serialize(request),
                Status = StatusConstant.Pending,
                CreatedDate = DateTimeOffset.UtcNow,
                RequestedBy = request.RequestedBy
            };
            await _branchRequestRepository.CreateAsync(updateRequest);

            return Ok(Result.Success());
        }

        [HttpPost("delete-branch-request")]
        public async Task<IActionResult> DeleteBranchRequestAsync([FromBody] long id)
        {
            var entity = await _branchRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                return NotFound(Result.Failure("Branch not found."));

            var requestedBy = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "system";

            var closeRequest = new BranchRequest
            {
                BranchId = id,
                RequestType = RequestTypeConstant.Delete,
                ProposedData = "{}",
                Status = StatusConstant.Pending,
                CreatedDate = DateTimeOffset.UtcNow,
                RequestedBy = requestedBy
            };
            await _branchRequestRepository.CreateAsync(closeRequest);

            return Ok(Result.Success());
        }

        [HttpPost("approve-branch-request")]
        public async Task<IActionResult> ApproveBranchRequestAsync([FromBody] ApproveBranchRequest request)
        {
            var branchRequest = await _branchRequestRepository.GetByIdAsync(request.RequestId);
            if (branchRequest == null)
                return BadRequest(Result.Failure(message: "Yêu cầu không tồn tại."));

            if (branchRequest.Status != StatusConstant.Pending)
                return BadRequest(Result.Failure(message: "Yêu cầu không hợp lệ hoặc đã được xử lý."));

            await using var transaction = await _branchRepository.BeginTransactionAsync();

            Result result = branchRequest.RequestType switch
            {
                RequestTypeConstant.Create => await HandleCreateAsync(branchRequest),
                RequestTypeConstant.Update => await HandleUpdateAsync(branchRequest),
                RequestTypeConstant.Delete => await HandleDeleteAsync(branchRequest),
                _ => Result.Failure("Loại yêu cầu không được hỗ trợ.")
            };

            if (!result.IsSuccess)
                return BadRequest(result);

            branchRequest.Status = StatusConstant.Approved;
            branchRequest.ApprovedBy = 1; // TODO: lấy từ context
            branchRequest.ApprovedDate = DateTime.UtcNow;
            branchRequest.ApprovalComment = request.Comment;
            await _branchRequestRepository.UpdateAsync(branchRequest);

            // Audit: thêm dòng lịch sử vào bảng lịch sử yêu cầu nếu cần

            await transaction.CommitAsync();

            return Ok(Result.Success());
        }

        [HttpPost("reject-branch-request")]
        public async Task<IActionResult> RejectBranchRequestAsync([FromBody] RejectBranchRequest request)
        {
            var branchRequest = await _branchRequestRepository.GetByIdAsync(request.RequestId);
            if (branchRequest == null)
                return NotFound(Result.Failure("Yêu cầu không tồn tại."));

            if (branchRequest.Status != StatusConstant.Pending)
                return BadRequest(Result.Failure("Yêu cầu không hợp lệ hoặc đã được xử lý."));

            branchRequest.Status = StatusConstant.Rejected;
            branchRequest.ApprovedBy = 1; // TODO: context
            branchRequest.ApprovedDate = DateTime.UtcNow;
            branchRequest.ApprovalComment = request.Comment;
            await _branchRequestRepository.UpdateAsync(branchRequest);

            return Ok(Result.Success("Từ chối yêu cầu thành công."));
        }

        [HttpPost("cancel-branch-request")]
        public async Task<IActionResult> CancelBranchRequestAsync([FromBody] long requestId)
        {
            var branchRequest = await _branchRequestRepository.GetByIdAsync(requestId);
            if (branchRequest == null)
                return NotFound(Result.Failure("Yêu cầu không tồn tại."));

            if (branchRequest.Status != StatusConstant.Pending)
                return BadRequest(Result.Failure("Chỉ có thể hủy yêu cầu đang chờ duyệt."));

            branchRequest.Status = StatusConstant.Canceled;
            await _branchRequestRepository.UpdateAsync(branchRequest);

            return Ok(Result.Success("Hủy yêu cầu thành công."));
        }

        private async Task<Result> HandleCreateAsync(BranchRequest request)
        {
            var data = request.ParseProposedData<CreateBranchRequest>();
            if (await _branchRepository.FindByCondition(x => x.Code == data.Code).AnyAsync())
                return Result.Failure("Mã chi nhánh đã tồn tại.");

            var entity = data.ToBranch();
            await _branchRepository.CreateAsync(entity);
            return Result.Success();
        }

        private async Task<Result> HandleUpdateAsync(BranchRequest request)
        {
            var data = request.ParseProposedData<UpdateBranchRequest>();
            var entity = await _branchRepository.FindByCondition(x => x.Id == data.Id).FirstOrDefaultAsync();
            if (entity == null)
                return Result.Failure("Chi nhánh không tồn tại.");

            var history = new BranchHistory
            {
                BranchId = entity.Id,
                OldData = JsonSerializer.Serialize(entity),
                CreatedDate = DateTime.UtcNow
            };
            await _branchHistoryRepository.CreateAsync(history);

            entity.Name = data.Name;
            entity.Address = data.Address;
            entity.Region = data.Region;
            entity.Manager = data.ManagerName;
            entity.EstablishedDate = data.EstablishedDate;
            await _branchRepository.UpdateAsync(entity);
            return Result.Success();
        }

        private async Task<Result> HandleDeleteAsync(BranchRequest request)
        {
            if (request.BranchId == null)
                return Result.Failure("Yêu cầu không chứa BranchId để xóa.");

            var entity = await _branchRepository.FindByCondition(x => x.Id == request.BranchId.Value).FirstOrDefaultAsync();
            if (entity == null)
                return Result.Failure("Chi nhánh không tồn tại.");

            await _branchRepository.DeleteAsync(entity);
            return Result.Success();
        }

        #endregion
    }
}