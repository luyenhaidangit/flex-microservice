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
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchRequestRepository _branchRequestRepository;
        private readonly IBranchHistoryRepository _branchHistoryRepository;
        private readonly IMapper _mapper;

        public BranchController(
            IBranchRepository branchRepository,
            IBranchRequestRepository branchRequestRepository,
            IBranchHistoryRepository branchHistoryRepository,
            IMapper mapper)
        {
            _branchRepository = branchRepository;
            _branchRequestRepository = branchRequestRepository;
            _branchHistoryRepository = branchHistoryRepository;
            _mapper = mapper;
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
            // 1. Check trùng mã trong các request tạo mới (Pending)
            var hasPending = await _branchRequestRepository
                .FindByCondition(x =>
                    x.Status == StatusConstant.Pending &&
                    x.RequestType == RequestTypeConstant.Create &&
                    x.ProposedData.Contains($"\"Code\":\"{request.Code}\""))
                .AnyAsync();

            if (hasPending)
            {
                return BadRequest(Result.Failure(message: "Đã có yêu cầu tạo chi nhánh với mã này đang chờ phê duyệt."));
            }

            // 2 .Kiểm tra mã chi nhánh đã tồn tại chưa
            var isExist = await _branchRepository.FindByCondition(x => x.Code == request.Code).AnyAsync();

            if (isExist)
            {
                return BadRequest(Result.Failure(message: "Mã chi nhánh đã tồn tại."));
            }

            // 3. Gửi yêu cầu
            var createRequest = request.ToCreateBranchRequest();

            await _branchRequestRepository.CreateAsync(createRequest);

            return Ok(Result.Success());
        }

        [HttpPost("approve-branch-request")]
        public async Task<IActionResult> ApproveBranchRequestAsync([FromQuery] ApproveBranchRequest request)
        {
            // 1. Validate
            var branchRequest = await _branchRequestRepository.GetByIdAsync(request.RequestId);
            if (request == null)
            {
                return NotFound(Result.Failure("Yêu cầu không tồn tại."));
            }

            if (branchRequest.Status != StatusConstant.Pending || branchRequest.RequestType != RequestTypeConstant.Create)
            {
                return BadRequest(Result.Failure("Yêu cầu không hợp lệ hoặc đã được xử lý."));
            }

            // 2. Parse ProposedData
            var data = branchRequest.ParseProposedData();

            // 3. Check trùng mã (double check lại tránh đua)
            var isExist = await _branchRepository.FindByCondition(x => x.Code == data.Code).AnyAsync();
            if (isExist)
            {
                return BadRequest(Result.Failure("Mã chi nhánh đã tồn tại."));
            }

            // 4. Duyệt request
            await using var transaction = await _branchRepository.BeginTransactionAsync();

            // Tạo chi nhánh
            var branch = data.ToBranch();
            await _branchRepository.CreateAsync(branch);

            // Cập nhật request
            branchRequest.Status = StatusConstant.Approved;
            branchRequest.ApprovedBy = 1;
            branchRequest.ApprovedDate = DateTime.UtcNow;
            branchRequest.ApprovalComment = request.Comment;
            await _branchRequestRepository.UpdateAsync(branchRequest);

            await transaction.CommitAsync();

            return Ok(Result.Success());
        }

        [HttpPost("create-branch")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateBranchRequest request)
        {
            var entity = _mapper.Map<Branch>(request);
            entity.Status = StatusConstant.Pending;
            await _branchRepository.CreateAsync(entity);

            var requestEntity = new BranchRequest
            {
                BranchId = entity.Id,
                RequestType = RequestTypeConstant.Create,
                ProposedData = JsonSerializer.Serialize(request),
                Status = StatusConstant.Pending,
                CreatedDate = DateTimeOffset.UtcNow,
                RequestedBy = request.RequestedBy
            };
            await _branchRequestRepository.CreateAsync(requestEntity);

            return Ok(Result.Success());
        }

        [HttpPost("update-branch")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateBranchRequest request)
        {
            var entity = await _branchRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (entity == null)
                return NotFound(Result.Failure(message: "Branch not found."));

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

        [HttpPost("close-branch")]
        public async Task<IActionResult> CloseAsync([FromBody] long id)
        {
            var entity = await _branchRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
            if (entity == null)
                return NotFound(Result.Failure(message: "Branch not found."));

            var closeRequest = new BranchRequest
            {
                BranchId = id,
                RequestType = RequestTypeConstant.Close,
                ProposedData = "{}",
                Status = StatusConstant.Pending,
                CreatedDate = DateTimeOffset.UtcNow,
                RequestedBy = User?.Identity?.Name ?? "system"
            };
            await _branchRequestRepository.CreateAsync(closeRequest);

            return Ok(Result.Success());
        }

        #endregion
    }
}