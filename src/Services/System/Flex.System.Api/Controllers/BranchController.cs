using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.System.Branch;
using Flex.Shared.SeedWork;
using Flex.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq;

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
            // branches
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

            // Request
            var requestQuery = _branchRequestRepository.FindByCondition(x => x.Status == "Pending");
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                requestQuery = requestQuery.Where(x => x.ProposedData.Contains(request.Keyword));
            }
            var pendingRequests = await requestQuery.ToListAsync();

            // Result
            var resultList = new List<BranchPagingDto>();

            resultList.AddRange(branches.Select(branch =>
            {
                var pending = pendingRequests.FirstOrDefault(r => r.BranchId == branch.Id);
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
            }));

            resultList.AddRange(pendingRequests
                .Where(r => r.BranchId == null || !branchDict.ContainsKey(r.BranchId.Value))
                .Select(r => new BranchPagingDto
                {
                    Id = 0,
                    Code = "--",
                    Name = "(Đề xuất tạo mới)",
                    Address = "--",
                    Region = "--",
                    Status = "Pending",
                    HasPendingRequest = true,
                    PendingRequestType = r.RequestType,
                    RequestedBy = r.RequestedBy,
                    RequestedDate = r.CreatedDate
                }));

            var sortedResult = resultList.OrderBy(x => x.Status != "Pending")
                                          .ThenByDescending(x => x.RequestedDate ?? DateTime.MinValue)
                                          .ToList();

            var pagedResult = sortedResult
                .Skip(((request.PageIndex ?? 1) - 1) * (request.PageSize ?? 10))
                .Take(request.PageSize ?? 10)
                .ToList();

            var response = new PagedResult<BranchPagingDto>
            {
                Items = pagedResult,
                TotalItems = resultList.Count,
                PageIndex = (request.PageIndex ?? 1),
                PageSize = (request.PageSize ?? resultList.Count)
            };

            return Ok(Result.Success(response));
        }

        //[HttpGet("get-branch-by-id")]
        //public async Task<IActionResult> GetByIdAsync([FromQuery] long id)
        //{
        //    var branch = await _branchRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
        //    if (branch == null)
        //        return NotFound(Result.Failure(message: "Branch not found."));

        //    var dto = _mapper.Map<BranchDto>(branch);
        //    return Ok(Result.Success(dto));
        //}

        //[HttpGet("get-branch-history")]
        //public async Task<IActionResult> GetHistoryAsync([FromQuery] long branchId)
        //{
        //    var histories = await _branchHistoryRepository.FindByCondition(x => x.BranchId == branchId)
        //        .OrderByDescending(x => x.ChangeDate)
        //        .ToListAsync();

        //    var result = _mapper.Map<List<BranchHistoryDto>>(histories);
        //    return Ok(Result.Success(result));
        //}

        //#endregion

        //#region Command

        //[HttpPost("create-branch")]
        //public async Task<IActionResult> CreateAsync([FromBody] CreateBranchRequest request)
        //{
        //    var entity = _mapper.Map<Branch>(request);
        //    entity.Status = "Pending";
        //    await _branchRepository.CreateAsync(entity);

        //    var requestEntity = new BranchRequest
        //    {
        //        BranchId = entity.Id,
        //        RequestType = "Create",
        //        ProposedData = System.Text.Json.JsonSerializer.Serialize(request),
        //        Status = "Pending",
        //        CreatedDate = DateTime.UtcNow
        //    };
        //    await _branchRequestRepository.CreateAsync(requestEntity);

        //    return Ok(Result.Success());
        //}

        //[HttpPost("update-branch")]
        //public async Task<IActionResult> UpdateAsync([FromBody] UpdateBranchRequest request)
        //{
        //    var entity = await _branchRepository.FindByCondition(x => x.Id == request.Id).FirstOrDefaultAsync();
        //    if (entity == null)
        //        return NotFound(Result.Failure(message: "Branch not found."));

        //    var updateRequest = new BranchRequest
        //    {
        //        BranchId = request.Id,
        //        RequestType = "Update",
        //        ProposedData = System.Text.Json.JsonSerializer.Serialize(request),
        //        Status = "Pending",
        //        CreatedDate = DateTime.UtcNow
        //    };
        //    await _branchRequestRepository.CreateAsync(updateRequest);

        //    return Ok(Result.Success());
        //}

        //[HttpPost("close-branch")]
        //public async Task<IActionResult> CloseAsync([FromBody] long id)
        //{
        //    var entity = await _branchRepository.FindByCondition(x => x.Id == id).FirstOrDefaultAsync();
        //    if (entity == null)
        //        return NotFound(Result.Failure(message: "Branch not found."));

        //    var closeRequest = new BranchRequest
        //    {
        //        BranchId = id,
        //        RequestType = "Close",
        //        ProposedData = "{}",
        //        Status = "Pending",
        //        CreatedDate = DateTime.UtcNow
        //    };
        //    await _branchRequestRepository.CreateAsync(closeRequest);

        //    return Ok(Result.Success());
        //}

        #endregion
    }
}
