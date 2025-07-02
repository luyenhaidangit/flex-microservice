using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.System.Branch;
using Flex.Shared.SeedWork;
using Flex.Shared.Constants.Common;
using Flex.Shared.SeedWork.General;
using Flex.System.Api.Entities;
using Flex.Shared.SeedWork.Workflow.Constants;

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
                .OrderBy(u => u.PendingAction == null ? 1 : 0)
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
        [HttpPost("request")]
        public async Task<IActionResult> ProcessBranchRequest([FromBody] BranchRequestDTO request)
        {
            // Validation chung
            if (request.RequestType == RequestTypeConstant.Create)
            {
                var existsInMaster = await _masterRepo.FindByCondition(x => x.Code == request.Code).AnyAsync();
                if (existsInMaster) return BadRequest(Result.Failure("Branch code already exists in master."));
            }
            else
            {
                var master = await _masterRepo.FindByCondition(x => x.Code == request.Code).FirstOrDefaultAsync();
                if (master == null) return BadRequest(Result.Failure("Branch does not exist."));
            }

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

            // Begin transaction
            await using var tx = await _headerRepo.BeginTransactionAsync();

            // 1. Insert header
            var header = new BranchRequestHeader
            {
                Action = request.RequestType,
                Status = RequestStatusConstant.Unauthorised,
                RequestedBy = User?.Identity?.Name ?? "anonymous",
                RequestedDate = DateTime.UtcNow
            };
            await _headerRepo.CreateAsync(header);

            // 2. Insert request data
            var data = new BranchRequestData
            {
                RequestId = header.Id,
                Code = request.Code,
                Name = request.Name,
                Address = request.Address
            };
            await _dataRepo.CreateAsync(data);

            await tx.CommitAsync();
            return Ok(Result.Success());
        }

        [HttpPost("process-branch-request")]
        public async Task<IActionResult> ProcessBranchRequest([FromBody] ApproveOrRejectRequest<long> request)
        {
            // ========== VALIDATION ==========
            var header = await _headerRepo.GetByIdAsync(request.Id);
            if (header == null) return BadRequest(Result.Failure("Request does not exist."));
            if (header.Status != RequestStatusConstant.Unauthorised)
                return BadRequest(Result.Failure("Request has already been processed."));

            var data = await _dataRepo.FindByCondition(x => x.RequestId == request.Id).FirstOrDefaultAsync();
            if (data == null) return BadRequest(Result.Failure("Request data is missing."));

            var currentUser = User?.Identity?.Name ?? "anonymous";

            // Process based on action type and approval decision
            if (request.IsApprove)
            {
                await using var tx = await _headerRepo.BeginTransactionAsync();

                switch (header.Action)
                {
                    case RequestTypeConstant.Create:
                        var newMaster = MappingProfile.MapToBranchMaster(data);
                        await _masterRepo.CreateAsync(newMaster);
                        var createAudit = MappingProfile.MapToAuditLog(newMaster.Id, AuditOperationConstant.CreateBranch,
                            null, newMaster, header.RequestedBy, currentUser);
                        await _auditRepo.CreateAsync(createAudit);
                        break;

                    case RequestTypeConstant.Update:
                        var master = await _masterRepo.FindByCondition(x => x.Code == data.Code).FirstOrDefaultAsync();
                        if (master == null) return BadRequest(Result.Failure("Branch no longer exists."));
                        var oldMaster = MappingProfile.CloneBranchMaster(master);

                        master.Name = data.Name;
                        master.Address = data.Address;
                        await _masterRepo.UpdateAsync(master);

                        var updateAudit = MappingProfile.MapToAuditLog(master.Id, AuditOperationConstant.UpdateBranch,
                            oldMaster, master, header.RequestedBy, currentUser);
                        await _auditRepo.CreateAsync(updateAudit);
                        break;

                    case RequestTypeConstant.Delete:
                        var deleteMaster = await _masterRepo.FindByCondition(x => x.Code == data.Code).FirstOrDefaultAsync();
                        if (deleteMaster == null) return BadRequest(Result.Failure("Branch no longer exists."));

                        var deleteAudit = MappingProfile.MapToAuditLog(deleteMaster.Id, AuditOperationConstant.DeleteBranch,
                            deleteMaster, null, header.RequestedBy, currentUser);
                        await _auditRepo.CreateAsync(deleteAudit);

                        await _masterRepo.DeleteAsync(deleteMaster);
                        break;
                }

                header.Status = RequestStatusConstant.Authorised;
                header.ApproveBy = currentUser;
                header.ApproveDate = DateTime.UtcNow;
                await _headerRepo.UpdateAsync(header);

                await tx.CommitAsync();
            }
            else
            {
                await using var tx = await _headerRepo.BeginTransactionAsync();

                header.Status = RequestStatusConstant.Rejected;
                header.ApproveBy = currentUser;
                header.ApproveDate = DateTime.UtcNow;
                header.Comments = (header.Comments ?? "") + $" | Rejected: {request.Comment}";
                await _headerRepo.UpdateAsync(header);

                var rejectAudit = MappingProfile.MapToAuditLog(header.Id, AuditOperationConstant.RejectRequest,
                    null, new { Reason = request.Comment }, header.RequestedBy, currentUser);
                await _auditRepo.CreateAsync(rejectAudit);

                await tx.CommitAsync();
            }

            return Ok(Result.Success());
        }

        [HttpPost("save-draft")]
        public async Task<IActionResult> SaveDraft([FromBody] BranchRequestDTO request)
        {
            // Kiểm tra đã có draft cho branch này chưa
            var hasDraft = await _headerRepo.FindAll()
                .Where(header => header.Status == RequestStatusConstant.Draft)
                .Join(
                    _dataRepo.FindAll().Where(data => data.Code == request.Code),
                    header => header.Id,
                    data => data.RequestId,
                    (header, data) => header
                )
                .AnyAsync();
            if (hasDraft) return BadRequest(Result.Failure("Branch already has a draft request."));

            // Begin transaction
            await using var tx = await _headerRepo.BeginTransactionAsync();

            // 1. Insert header (Draft)
            var header = new BranchRequestHeader
            {
                Action = request.RequestType,
                Status = RequestStatusConstant.Draft,
                RequestedBy = User?.Identity?.Name ?? "anonymous",
                RequestedDate = DateTime.UtcNow
            };
            await _headerRepo.CreateAsync(header);

            // 2. Insert request data
            var data = new BranchRequestData
            {
                RequestId = header.Id,
                Code = request.Code,
                Name = request.Name,
                Address = request.Address
            };
            await _dataRepo.CreateAsync(data);

            await tx.CommitAsync();
            return Ok(Result.Success());
        }
        #endregion
    }
}
