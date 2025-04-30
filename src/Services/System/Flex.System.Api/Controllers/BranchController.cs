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

            // Create header
            var header = MappingProfile.MapToBranchRequestHeader(request);
            await _headerRepo.CreateAsync(header);

            // Create request data
            var requestData = MappingProfile.MapToBranchRequestData(request, header.Id);
            await _dataRepo.CreateAsync(requestData);

            await transaction.CommitAsync();
            // ========== END TRANSACTION ==========

            return Ok(Result.Success());
        }

        [HttpPost("approve-branch-request")]
        public async Task<IActionResult> ApproveBranchRequest([FromBody] ApproveBranchRequest request)
        {
            // ========== BEGIN TRANSACTION ==========
            await using var transaction = await _headerRepo.BeginTransactionAsync();

            // 1. Lấy header
            var header = await _headerRepo.GetByIdAsync(request.RequestId);
            if (header == null) return NotFound(Result.Failure("Yêu cầu không tồn tại."));
            if (header.Status != RequestStatusConstant.Unauthorised)
                return BadRequest(Result.Failure("Yêu cầu đã được xử lý."));

            // 2. Lấy dữ liệu chi tiết
            var data = await _dataRepo.FindByCondition(x => x.RequestId == request.RequestId).FirstOrDefaultAsync();
            if (data == null) return BadRequest(Result.Failure("Không có dữ liệu chi nhánh."));

            // 3. Ghi vào BranchMaster
            var master = new BranchMaster
            {
                Code = data.Code,
                Name = data.Name,
                Address = data.Address,
                Status = BranchStatusConstant.Active
            };
            await _masterRepo.CreateAsync(master);

            // 4. Cập nhật Header
            header.Status = RequestStatusConstant.Authorised;
            header.CheckerId = request.ApprovedBy;
            header.CheckerDate = DateTime.UtcNow;
            await _headerRepo.UpdateAsync(header);

            // 5. Ghi Audit Log
            var audit = new BranchAuditLog
            {
                EntityId = master.Id,
                Operation = AuditOperationConstant.Create,
                OldValue = null,
                NewValue = JsonSerializer.Serialize(master),
                UserId = request.ApprovedBy,
                LogDate = DateTime.UtcNow
            };
            await _auditRepo.CreateAsync(audit);

            await transaction.CommitAsync();
            // ========== END TRANSACTION ==========

            return Ok(Result.Success());
        }

        [HttpPost("reject-branch-request")]
        public async Task<IActionResult> Reject(long id, [FromQuery] string checkerId, [FromQuery] string reason)
        {
            var header = await _headerRepo.GetByIdAsync(id);
            if (header == null) return NotFound("Request not found.");
            if (header.Status != "UNA") return BadRequest("Request already processed.");

            header.Status = "REJ";
            header.CheckerId = checkerId;
            header.CheckerDate = DateTime.UtcNow;
            header.Comments = (header.Comments ?? "") + $" | Rejected: {reason}";

            await _headerRepo.UpdateAsync(header);
            await _headerRepo.SaveChangesAsync();

            var audit = new BranchAuditLog
            {
                EntityId = id,
                Operation = "REJECT",
                UserId = checkerId,
                LogDate = DateTime.UtcNow
            };

            await _auditRepo.CreateAsync(audit);
            await _auditRepo.SaveChangesAsync();

            return Ok("Rejected successfully.");
        }
        #endregion
    }
}