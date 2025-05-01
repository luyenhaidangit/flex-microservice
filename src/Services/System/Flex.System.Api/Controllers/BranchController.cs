using Microsoft.AspNetCore.Mvc;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.System.Branch;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using Flex.Shared.Constants.Common;
using Flex.System.Api.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq.Dynamic.Core;

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
        private readonly IBranchRequestHeaderRepository _headerRepo;
        private readonly IBranchRequestDataRepository _dataRepo;
        private readonly IBranchMasterRepository _masterRepo;
        private readonly IBranchAuditLogRepository _auditRepo;

        public BranchController(
            IBranchRequestHeaderRepository headerRepo,
            IBranchRequestDataRepository dataRepo,
            IBranchMasterRepository masterRepo,
            IBranchAuditLogRepository auditRepo)
        {
            _headerRepo = headerRepo;
            _dataRepo = dataRepo;
            _masterRepo = masterRepo;
            _auditRepo = auditRepo;
        }

        #region Query
        [HttpGet("get-branches-paging")]
        public async Task<IActionResult> GetPagingBranchesAsync([FromQuery] GetBranchesPagingRequest request)
        {
            var headerQuery = _headerRepo.FindAll();
            var dataQuery = _dataRepo.FindAll();
            var masterQuery = _masterRepo.FindAll();

            // --------- 1. Pending requests (sub-query) ---------
            IQueryable<PendingInfo> pendingQ =
                from hdr in headerQuery
                join data in dataQuery
                      on hdr.Id equals data.RequestId
                where hdr.Status == "UNA"
                select new PendingInfo
                {
                    Code = data.Code,
                    Action = hdr.Action,
                    RequestedDate = hdr.RequestedDate,
                    Name = data.Name,
                    Address = data.Address
                };

            // --------- 2. Phần LEFT JOIN với BRANCH_MASTER ---------
            IQueryable<BranchDto> masterWithPendingQ =
                from bm in masterQuery
                join p in pendingQ          // left join
                      on bm.Code equals p.Code into lj
                from p in lj.DefaultIfEmpty()
                select new BranchDto(
                    bm.Id,
                    bm.Code,
                    bm.Name,
                    bm.Address,
                    p!.Action,             // null khi không có pending
                    p!.RequestedDate);

            // --------- 3. Những CREATE chưa có trên master (UNION) ---------
            IQueryable<BranchDto> createOnlyQ =
                from p in pendingQ
                where p.Action == "CREATE"
                   && !masterQuery.Any(m => m.Code == p.Code)
                select new BranchDto(
                    null,                  // Id = NULL
                    p.Code,
                    p.Name,
                    p.Address,
                    "CREATE",
                    p.RequestedDate);

            // --------- 4. Hợp nhất + FILTER theo keyword ---------
            IQueryable<BranchDto> unionQ = masterWithPendingQ.Concat(createOnlyQ);

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                string kw = request.Keyword.Trim();
                unionQ = unionQ.Where(x =>
                    x.Code.Contains(kw) ||
                    x.Name.Contains(kw));
            }

            // --------- 5. SORT (pending trước, mới nhất trước, sau đó Code) ---------
            unionQ = unionQ
                .OrderBy(x => x.PendingAction == null)          // false (có pending) < true
                .ThenByDescending(x => x.RequestedDate)
                .ThenBy(x => x.Code);

            // --------- 6. PAGING & kết quả ---------
            int total = await unionQ.CountAsync();

            List<BranchDto> items = await unionQ
                .Skip((request.PageIndex.Value - 1) * request.PageSize.Value)
                .Take(request.PageSize.Value)
                .ToListAsync();

            var response = new Flex.Shared.SeedWork.PagedResult<BranchDto>
            {
                Items = items,
                TotalItems = items.Count,
                PageIndex = request.PageIndex.Value,
                PageSize = request.PageSize.Value
            };

            return Ok(Result.Success(data: response));
        }
        #endregion

        #region Command
        [HttpPost("create-branch-request")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateBranchRequest request)
        {
            // ========== VALIDATION ==========
            var existsInMaster = await _masterRepo.FindByCondition(x => x.Code == request.Code).AnyAsync();
            if (existsInMaster) return BadRequest(Result.Failure(message: "Branch code already exists in master."));
            var existsPendingRequest = await _dataRepo.FindAll().Where(data => data.Code == request.Code)
                .Join(
                    _headerRepo.FindAll().Where(header => header.Status == RequestStatusConstant.Unauthorised),
                    data => data.RequestId,
                    header => header.Id,
                    (data, header) => data
                )
                .AnyAsync();
            if (existsPendingRequest) return BadRequest(Result.Failure(message: "Branch code already has a pending request."));

            // ========== BEGIN TRANSACTION ==========
            await using var transaction = await _headerRepo.BeginTransactionAsync();

            // 1. Insert header Unauthorised
            var header = MappingProfile.MapToBranchRequestHeader(request);
            header.RequestedBy = User?.Identity?.Name ?? "anonymous";
            await _headerRepo.CreateAsync(header);

            // 2. Create request data
            var requestData = MappingProfile.MapToBranchRequestData(request, header.Id);
            await _dataRepo.CreateAsync(requestData);

            await transaction.CommitAsync();
            // ========== END TRANSACTION ==========

            return Ok(Result.Success());
        }

        [HttpPost("approve-branch-request")]
        public async Task<IActionResult> ApproveBranchRequest([FromBody] ApproveBranchRequest request)
        {
            // ========== VALIDATION ==========
            // 1. Request exits
            var header = await _headerRepo.GetByIdAsync(request.RequestId);
            if (header == null) return BadRequest(Result.Failure(message: "Request does not exist."));
            if (header.Status != RequestStatusConstant.Unauthorised) return BadRequest(Result.Failure(message: "Request has already been processed."));

            var data = await _dataRepo.FindByCondition(x => x.RequestId == request.RequestId).FirstOrDefaultAsync();
            if (data == null) return BadRequest(Result.Failure(message: "Branch request data is missing."));

            // ========== BEGIN TRANSACTION ==========
            await using var transaction = await _headerRepo.BeginTransactionAsync();

            // 1. Insert BranchMaster
            var master = MappingProfile.MapToBranchMaster(data);
            await _masterRepo.CreateAsync(master);

            // 2. Update Header
            header.Status = RequestStatusConstant.Authorised;
            header.ApproveBy = User?.Identity?.Name ?? "anonymous";
            header.ApproveDate = DateTime.UtcNow;
            await _headerRepo.UpdateAsync(header);

            // 3. Insert Audit Log
            var audit = MappingProfile.MapToAuditLog(master.Id, AuditOperationConstant.CreateBranch, null, master, header.RequestedBy, header.ApproveBy);
            await _auditRepo.CreateAsync(audit);

            await transaction.CommitAsync();
            // ========== END TRANSACTION ==========

            return Ok(Result.Success());
        }

        [HttpPost("reject-branch-request")]
        public async Task<IActionResult> RejectBranchRequest([FromBody] RejectBranchRequest request)
        {
            // ========== VALIDATION ==========
            var header = await _headerRepo.GetByIdAsync(request.RequestId);
            if (header == null) return BadRequest(Result.Failure(message: "Request does not exist."));
            if (header.Status != RequestStatusConstant.Unauthorised) return BadRequest(Result.Failure(message: "Request has already been processed."));

            var currentUser = User?.Identity?.Name ?? "anonymous";

            // ========== BEGIN TRANSACTION ==========
            await using var transaction = await _headerRepo.BeginTransactionAsync();

            // 1. Update Status Reject
            header.Status = RequestStatusConstant.Rejected;
            header.ApproveBy = currentUser;
            header.ApproveDate = DateTime.UtcNow;
            header.Comments = (header.Comments ?? "") + $" | Rejected: {request.Comment}";
            await _headerRepo.UpdateAsync(header);

            // 2. Insert log
            var audit = MappingProfile.MapToAuditLog(header.Id, AuditOperationConstant.RejectRequest, null, new { Reason = request.Comment }, header.RequestedBy, header.ApproveBy);
            await _auditRepo.CreateAsync(audit);

            await transaction.CommitAsync();
            // ========== END TRANSACTION ==========

            return Ok(Result.Success());
        }

        [HttpPost("update-branch-request")]
        public async Task<IActionResult> UpdateBranchRequest([FromBody] UpdateBranchRequest request)
        {
            // ========== VALIDATION ==========
            var master = await _masterRepo.FindByCondition(x => x.Code == request.Code).FirstOrDefaultAsync();
            if (master == null) return BadRequest(Result.Failure("Branch does not exist."));

            var hasPending = await _headerRepo.FindAll()
                .Where(header => header.Status == RequestStatusConstant.Unauthorised)
                .Join(
                    _dataRepo.FindAll().Where(data => data.Code == request.Code),
                    header => header.Id,
                    data => data.RequestId,
                    (header, data) => header
                )
                .AnyAsync();
            if (hasPending) return BadRequest(Result.Failure("Branch already has a pending request."));

            // ========== BEGIN TRANSACTION ==========
            await using var tx = await _headerRepo.BeginTransactionAsync();

            // 1. Insert header Unauthorised
            var header = MappingProfile.MapToBranchRequestHeader(request);
            header.RequestedBy = User?.Identity?.Name ?? "anonymous";
            await _headerRepo.CreateAsync(header);

            // 2. Create request data
            var data = MappingProfile.MapToBranchRequestData(request, header.Id);
            await _dataRepo.CreateAsync(data);

            await tx.CommitAsync();
            return Ok(Result.Success());
        }

        [HttpPost("approve-update-request")]
        public async Task<IActionResult> ApproveUpdateRequest([FromBody] ApproveBranchRequest request)
        {
            // ========== VALIDATION ==========
            var header = await _headerRepo.GetByIdAsync(request.RequestId);
            if (header == null || header.Action != RequestTypeConstant.Update) return BadRequest(Result.Failure("Update-request not found."));
            if (header.Status != RequestStatusConstant.Unauthorised) return BadRequest(Result.Failure("Request has already been processed."));

            var data = await _dataRepo.FindByCondition(x => x.RequestId == request.RequestId).FirstOrDefaultAsync();
            if (data == null) return BadRequest(Result.Failure("Request data is missing."));
            var master = await _masterRepo.FindByCondition(x => x.Code == data.Code).FirstOrDefaultAsync();
            if (master == null) return BadRequest(Result.Failure("Branch no longer exists."));

            var oldMaster = MappingProfile.CloneBranchMaster(master);
            // ========== BEGIN TRANSACTION ==========
            await using var tx = await _headerRepo.BeginTransactionAsync();

            // 1. Update master
            master.Name = data.Name;
            master.Address = data.Address;
            await _masterRepo.UpdateAsync(master);

            // 2. Update header
            header.Status = RequestStatusConstant.Authorised;
            header.ApproveBy = User?.Identity?.Name ?? "anonymous";
            header.ApproveDate = DateTime.UtcNow;
            await _headerRepo.UpdateAsync(header);

            // 3. Insert Audit Log
            var audit = MappingProfile.MapToAuditLog(master.Id, AuditOperationConstant.UpdateBranch, oldMaster, master, header.RequestedBy, header.ApproveBy);
            await _auditRepo.CreateAsync(audit);

            await tx.CommitAsync();
            // ========== END TRANSACTION ==========

            return Ok(Result.Success());
        }

        [HttpPost("reject-update-request")]
        public async Task<IActionResult> RejectUpdateRequest([FromBody] RejectBranchRequest request)
        {
            // ========== VALIDATION ==========
            var header = await _headerRepo.GetByIdAsync(request.RequestId);
            if (header == null || header.Action != RequestTypeConstant.Update)
                return BadRequest(Result.Failure("Update request does not exist."));
            if (header.Status != RequestStatusConstant.Unauthorised)
                return BadRequest(Result.Failure("Request has already been processed."));

            var currentUser = User?.Identity?.Name ?? "anonymous";

            // ========== BEGIN TRANSACTION ==========
            await using var transaction = await _headerRepo.BeginTransactionAsync();

            // 1. Update status to Rejected
            header.Status = RequestStatusConstant.Rejected;
            header.ApproveBy = currentUser;
            header.ApproveDate = DateTime.UtcNow;
            header.Comments = (header.Comments ?? "") + $" | Rejected: {request.Comment}";
            await _headerRepo.UpdateAsync(header);

            // 2. Insert Audit Log
            var audit = MappingProfile.MapToAuditLog(header.Id, AuditOperationConstant.RejectRequest, null, new { Reason = request.Comment }, header.RequestedBy, header.ApproveBy);
            await _auditRepo.CreateAsync(audit);

            await transaction.CommitAsync();
            // ========== END TRANSACTION ==========

            return Ok(Result.Success());
        }

        [HttpPost("delete-branch-request")]
        public async Task<IActionResult> DeleteBranchRequest([FromBody] DeleteBranchRequest request)
        {
            // ========== VALIDATION ==========
            var master = await _masterRepo.FindByCondition(x => x.Code == request.Code).FirstOrDefaultAsync();
            if (master == null) return BadRequest(Result.Failure("Branch does not exist."));

            var hasPending = await _headerRepo.FindAll()
                .Where(header => header.Status == RequestStatusConstant.Unauthorised)
                .Join(
                    _dataRepo.FindAll().Where(data => data.Code == request.Code),
                    header => header.Id,
                    data => data.RequestId,
                    (header, data) => header
                )
                .AnyAsync();
            if (hasPending) return BadRequest(Result.Failure("Branch already has a pending request."));

            // ========== BEGIN TRANSACTION ==========
            await using var tx = await _headerRepo.BeginTransactionAsync();

            var header = MappingProfile.MapToBranchRequestHeader(request);
            header.RequestedBy = User?.Identity?.Name ?? "anonymous";
            await _headerRepo.CreateAsync(header);

            var data = MappingProfile.MapToBranchRequestData(request, header.Id);
            await _dataRepo.CreateAsync(data);

            await tx.CommitAsync();
            return Ok(Result.Success());
        }

        [HttpPost("approve-delete-request")]
        public async Task<IActionResult> ApproveDeleteRequest([FromBody] ApproveBranchRequest request)
        {
            // ========== VALIDATION ==========
            var header = await _headerRepo.GetByIdAsync(request.RequestId);
            if (header == null || header.Action != RequestTypeConstant.Delete)
                return BadRequest(Result.Failure("Delete request not found."));
            if (header.Status != RequestStatusConstant.Unauthorised)
                return BadRequest(Result.Failure("Request has already been processed."));

            var data = await _dataRepo.FindByCondition(x => x.RequestId == request.RequestId).FirstOrDefaultAsync();
            if (data == null) return BadRequest(Result.Failure("Request data is missing."));

            var master = await _masterRepo.FindByCondition(x => x.Code == data.Code).FirstOrDefaultAsync();
            if (master == null) return BadRequest(Result.Failure("Branch no longer exists."));

            var oldMaster = MappingProfile.CloneBranchMaster(master);

            // ========== BEGIN TRANSACTION ==========
            await using var tx = await _headerRepo.BeginTransactionAsync();

            await _masterRepo.DeleteAsync(master);

            header.Status = RequestStatusConstant.Authorised;
            header.ApproveBy = User?.Identity?.Name ?? "anonymous";
            header.ApproveDate = DateTime.UtcNow;
            await _headerRepo.UpdateAsync(header);

            var audit = MappingProfile.MapToAuditLog(master.Id, AuditOperationConstant.DeleteBranch, oldMaster, master, header.RequestedBy, header.ApproveBy);
            await _auditRepo.CreateAsync(audit);

            await tx.CommitAsync();
            return Ok(Result.Success());
        }

        [HttpPost("reject-delete-request")]
        public async Task<IActionResult> RejectDeleteRequest([FromBody] RejectBranchRequest request)
        {
            // ========== VALIDATION ==========
            var header = await _headerRepo.GetByIdAsync(request.RequestId);
            if (header == null || header.Action != RequestTypeConstant.Delete)
                return BadRequest(Result.Failure("Delete request does not exist."));
            if (header.Status != RequestStatusConstant.Unauthorised)
                return BadRequest(Result.Failure("Request has already been processed."));

            var currentUser = User?.Identity?.Name ?? "anonymous";

            // ========== BEGIN TRANSACTION ==========
            await using var transaction = await _headerRepo.BeginTransactionAsync();

            header.Status = RequestStatusConstant.Rejected;
            header.ApproveBy = currentUser;
            header.ApproveDate = DateTime.UtcNow;
            header.Comments = (header.Comments ?? "") + $" | Rejected: {request.Comment}";
            await _headerRepo.UpdateAsync(header);

            await transaction.CommitAsync();
            return Ok(Result.Success());
        }
        #endregion
    }

    public class BranchDto
    {
        public long? Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string? PendingAction { get; set; }
        public DateTime? RequestedDate { get; set; }

        public BranchDto(
        long? id,
        string code,
        string name,
        string address,
        string? pendingAction,
        DateTime? requestedDate)
        {
            Id = id;
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            PendingAction = pendingAction;
            RequestedDate = requestedDate;
        }

    }

    public sealed record PendingInfo
    {
        public string Code { get; init; } = default!;
        public string Action { get; init; } = default!;
        public DateTime? RequestedDate { get; init; }
        public string Name { get; init; } = default!;
        public string? Address { get; init; }
    }
}