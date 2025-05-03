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
        public async Task<IActionResult> GetPagingBranchesAsync([FromQuery] GetBranchesPagingRequest request)
        {
            // ========== PAGING ==========
            var headerQuery = _headerRepo.FindAll();
            var dataQuery = _dataRepo.FindAll();
            var masterQuery = _masterRepo.FindAll();

            // 1. Get pending request header (sub-query)
            var pendingQuery =
                from h in headerQuery.Where(x => x.Status == RequestStatusConstant.Unauthorised)
                from d in dataQuery.Where(d => d.RequestId == h.Id)
                select new
                {
                    d.Code,
                    d.Name,
                    d.Address,
                    h.Action,
                    h.Id,
                    h.RequestedDate
                };

            // 2. Left join: master -> pending
            var masterPart =
                from m in masterQuery
                join p in pendingQuery on m.Code equals p.Code into lj
                from p in lj.DefaultIfEmpty()
                select new
                {
                    Id = (long?)m.Id,
                    m.Code,
                    Name = m.Name,
                    Address = m.Address,
                    PendingAction = (string?)p.Action,
                    RequestId = (long?)p.Id,
                    RequestedDate = (DateTime?)p.RequestedDate
                };

            // 3. Get create request pending
            var createPart =
                from p in pendingQuery
                where p.Action == RequestTypeConstant.Create
                && !masterQuery.Any(m => m.Code == p.Code)
                select new
                {
                    Id = (long?)null,
                    p.Code,
                    p.Name,
                    p.Address,
                    PendingAction = p.Action,
                    RequestId = (long?)p.Id,
                    RequestedDate = (DateTime?)p.RequestedDate
                };

            // 4. Union masterPart and createPart
            var unionQ = masterPart.Concat(createPart);

            // 5. Filter keyword
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim();
                unionQ = unionQ.Where(x => x.Code.Contains(kw) || x.Name.Contains(kw));
            }

            // 6. Sort: Pending -> Time request pending -> Id
            unionQ = unionQ
                .OrderBy(x => x.PendingAction == null)
                .ThenByDescending(x => x.RequestedDate)
                .ThenBy(x => x.Id);

            // 7. Total items
            var total = await unionQ.CountAsync();

            if (request.PageIndex == null) request.PageIndex = 1;
            if (request.PageSize == null) request.PageSize = total;

            int pageIndex = request.PageIndex == null ? 1 : request.PageIndex.Value;
            int pageSize = request.PageSize == null ? total : request.PageSize.Value;

            // 8. Paging + select data
            var items = await unionQ
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BranchPagingDto(
                    x.Id,
                    x.Code,
                    x.Name,
                    x.Address,
                    x.RequestId,
                    x.PendingAction,
                    x.RequestedDate))
                .ToListAsync();

            var resp = Shared.SeedWork.PagedResult<BranchPagingDto>.Create(pageIndex,pageSize,total,items);
            // ========== PAGING ==========

            return Ok(Result.Success(data: resp));
        }

        [HttpGet("get-pending-update-request")]
        public async Task<IActionResult> GetPendingUpdateRequest([FromQuery] string code)
        {
            var pendingRequest = await _headerRepo.FindAll()
                .Where(h => h.Status == RequestStatusConstant.Unauthorised 
                       && h.Action == RequestTypeConstant.Update)
                .Join(
                    _dataRepo.FindAll().Where(d => d.Code == code),
                    header => header.Id,
                    data => data.RequestId,
                    (header, data) => new GetPendingUpdateRequestResponse
                    {
                        RequestId = header.Id,
                        Code = data.Code,
                        Name = data.Name,
                        Address = data.Address,
                        RequestedBy = header.RequestedBy,
                        RequestedDate = header.RequestedDate
                    })
                .FirstOrDefaultAsync();

            if (pendingRequest == null)
            {
                return NotFound(Result.Failure("No pending update request found for this branch."));
            }

            return Ok(Result.Success(data: pendingRequest));
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

            // Map current branch info to request
            request.Name = master.Name;
            request.Address = master.Address;

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
}
