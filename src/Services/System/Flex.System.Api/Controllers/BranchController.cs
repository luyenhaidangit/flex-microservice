using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.System.Branch;
using Flex.Shared.SeedWork;
using Flex.Shared.Constants.Common;

namespace Flex.System.Api.Controllers
{
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
        public async Task<IActionResult> GetPagingBranchesAsync([FromQuery] GetBranchesPagingRequest r)
        {
            // shorthand
            var hdrQ = _headerRepo.FindAll();   // BRANCH_REQUEST_HEADER
            var dataQ = _dataRepo.FindAll();     // BRANCH_REQUEST_DATA
            var masterQ = _masterRepo.FindAll();   // BRANCH_MASTER

            // 1. Pending (sub-query)
            var pendingQ =
                from h in hdrQ
                where h.Status == "UNA"
                from d in dataQ
                    .Where(d => d.RequestId == h.Id)   // tránh join lồng
                select new
                {
                    d.Code,
                    d.Name,
                    d.Address,
                    h.Action,
                    h.RequestedDate
                };

            // 2. LEFT JOIN master ↔ pending
            var masterPart =
                from m in masterQ
                join p in pendingQ on m.Code equals p.Code into lj
                from p in lj.DefaultIfEmpty()
                select new               // **anonymous type**, chưa tạo DTO
                {
                    Id = (long?)m.Id,
                    m.Code,
                    Name = m.Name,
                    Address = m.Address,
                    PendingAction = (string?)p.Action,
                    RequestedDate = (DateTime?)p.RequestedDate
                };

            // 3. CREATE chưa có trong master
            var createPart =
            from p in pendingQ
            where p.Action == "CREATE"
            && !masterQ.Any(m => m.Code == p.Code)
            select new
            {
                Id = (long?)null,
                p.Code,
                p.Name,
                p.Address,
                PendingAction = p.Action,          // ← KHÔNG dùng hằng "CREATE"
                RequestedDate = (DateTime?)p.RequestedDate
            };

            // 4. UNION rồi mới projection
            var unionQ = masterPart.Concat(createPart);

            // 5. FILTER keyword (nếu có)
            if (!string.IsNullOrWhiteSpace(r.Keyword))
            {
                var kw = r.Keyword.Trim();
                unionQ = unionQ.Where(x => x.Code.Contains(kw) || x.Name.Contains(kw));
            }

            // 6. SORT
            unionQ = unionQ
                .OrderBy(x => x.PendingAction == null)   // pending trước
                .ThenByDescending(x => x.RequestedDate)
                .ThenBy(x => x.Code);

            // 7. Tính total trước khi paging
            var total = await unionQ.CountAsync();

            int pageIndex = r.PageIndex is > 0 ? r.PageIndex.Value : 1;
            int pageSize = r.PageSize is > 0 && r.PageSize <= 100
                            ? r.PageSize.Value
                            : 20;

            // 8. Paging + chuyển sang DTO ngay trước khi lấy data
            var items = await unionQ
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BranchDto(          // **project sang DTO ở đây**
                    x.Id,
                    x.Code,
                    x.Name,
                    x.Address,
                    x.PendingAction,
                    x.RequestedDate))
                .ToListAsync();

            var resp = new Flex.Shared.SeedWork.PagedResult<BranchDto>
            {
                Items = items,
                TotalItems = total,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return Ok(Result.Success(data: resp));
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