using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Flex.System.Api.Entities;
using Flex.System.Api.Repositories.Interfaces;
using Flex.Shared.DTOs.System.Branch;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Flex.Shared.Constants.Common;
using Flex.Shared.Constants.System.Branch;

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
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IBranchRequestHeaderRepository _headerRepo;
        private readonly IBranchRequestDataRepository _dataRepo;
        private readonly IBranchMasterRepository _masterRepo;
        private readonly IBranchAuditLogRepository _auditRepo;

        public BranchController(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IBranchRequestHeaderRepository headerRepo,
            IBranchRequestDataRepository dataRepo,
            IBranchMasterRepository masterRepo,
            IBranchAuditLogRepository auditRepo)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;

            _headerRepo = headerRepo;
            _dataRepo = dataRepo;
            _masterRepo = masterRepo;
            _auditRepo = auditRepo;
        }

        #region Query

        #endregion

        #region Command
        [HttpPost("create-branch-request")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateBranchRequest request)
        {
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
            var header = await _headerRepo.GetByIdAsync(request.RequestId);
            if (header == null) return BadRequest(Result.Failure(message: "Request does not exist."));
            if (header.Status != RequestStatusConstant.Unauthorised) return BadRequest(Result.Failure(message: "Request has already been processed."));

            var data = await _dataRepo.FindByCondition(x => x.RequestId == request.RequestId).FirstOrDefaultAsync();
            if (data == null) return BadRequest(Result.Failure(message: "Branch request data is missing."));

            // ========== BEGIN TRANSACTION ==========
            await using var transaction = await _headerRepo.BeginTransactionAsync();

            // 1. Insert BranchMaster
            var master = MappingProfile.MapToBranchMaster(data);
            master.Status = BranchStatusConstant.Active;
            await _masterRepo.CreateAsync(master);

            // 2. Update Header
            header.Status = RequestStatusConstant.Authorised;
            header.ApproveBy = User?.Identity?.Name ?? "anonymous";
            header.ApproveDate = DateTime.UtcNow;
            await _headerRepo.UpdateAsync(header);

            // 3. Insert Audit Log
            var audit = MappingProfile.MapToCreateAuditLog(master,header.RequestedBy,header.ApproveBy);
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
            if (header == null)
                return BadRequest(Result.Failure(message: "Request does not exist."));
            if (header.Status != RequestStatusConstant.Unauthorised)
                return BadRequest(Result.Failure(message: "Request has already been processed."));

            var currentUser = User?.Identity?.Name ?? "anonymous";

            // ========== BEGIN TRANSACTION ==========
            await using var transaction = await _headerRepo.BeginTransactionAsync();

            // 1. Cập nhật trạng thái bị từ chối
            header.Status = RequestStatusConstant.Rejected;
            header.ApproveBy = currentUser;
            header.ApproveDate = DateTime.UtcNow;
            header.Comments = (header.Comments ?? "") + $" | Rejected: {request.Comment}";
            await _headerRepo.UpdateAsync(header);

            // 2. Ghi log
            var audit = MappingProfile.MapToRejectAuditLog(
                request.RequestId,
                header.RequestedBy,
                currentUser,
                request.Comment
            );
            await _auditRepo.CreateAsync(audit);

            await transaction.CommitAsync();
            // ========== END TRANSACTION ==========

            return Ok(Result.Success());
        }
        #endregion
    }
}