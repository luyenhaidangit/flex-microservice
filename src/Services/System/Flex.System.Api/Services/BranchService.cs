using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Flex.System.Api.Entities;
using Flex.System.Api.Models;
using Flex.System.Api.Repositories.Interfaces;
using Flex.System.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Flex.Shared.Constants.Common;
using Flex.System.Api.Persistence;

namespace Flex.System.Api.Services
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchRequestRepository _branchRequestRepository;
        private readonly IUserService _userService;
        private readonly SystemDbContext _dbContext;

        public BranchService(
            IBranchRepository branchRepository,
            IBranchRequestRepository branchRequestRepository,
            IUserService userService,
            SystemDbContext dbContext)
        {
            _branchRepository = branchRepository;
            _branchRequestRepository = branchRequestRepository;
            _userService = userService;
            _dbContext = dbContext;
        }

        #region Query
        public async Task<PagedResult<BranchListItemDto>> GetApprovedBranchesPagedAsync(GetBranchPagingRequest request)
        {
            return await _branchRepository.GetApprovedPagedAsync(request);
        }

        public async Task<BranchDto> GetApprovedBranchByCodeAsync(string code)
        {
            var entity = await _branchRepository.GetByCodeAsync(code);
            if (entity == null || entity.Status != StatusConstant.Approved)
            {
                throw new Exception($"Branch with code '{code}' not found or not approved.");
            }

            return new BranchDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description ?? string.Empty,
                Status = entity.Status,
                IsActive = entity.IsActive
            };
        }

        public async Task<List<BranchChangeHistoryDto>> GetApprovedBranchChangeHistoryAsync(string code)
        {
            // Implementation for change history
            throw new NotImplementedException();
        }
        #endregion

        #region Command
        public async Task<long> CreateBranchRequestAsync(CreateBranchRequestDto request)
        {
            // ===== Validation =====
            if (await _branchRepository.ExistsByCodeAsync(request.Code))
            {
                throw new Exception($"Branch with code '{request.Code}' already exists.");
            }

            // ===== Check duplicate pending request via view =====
            var hasPending = await _dbContext.ProposedBranches
                .AsNoTracking()
                .AnyAsync(v => v.Status == RequestStatusConstant.Unauthorised && v.Code == request.Code);
            if (hasPending)
            {
                throw new Exception($"Pending branch request already exists for code '{request.Code}'.");
            }

            // ===== Create request =====
            var requester = _userService.GetCurrentUsername() ?? "anonymous";
            var branchRequest = new BranchRequest
            {
                Action = RequestTypeConstant.Create,
                Status = RequestStatusConstant.Unauthorised,
                EntityId = 0,
                MakerId = requester,
                RequestedDate = DateTime.UtcNow,
                Comments = "Yêu cầu thêm mới vai trò.",
                RequestedData = JsonSerializer.Serialize(request),
            };

            await _branchRequestRepository.CreateAsync(branchRequest);
            return branchRequest.Id;
        }

        public async Task<long> CreateUpdateBranchRequestAsync(string code, UpdateBranchRequestDto dto)
        {
            // ===== Validation =====
            var existingEntity = await _branchRepository.GetByCodeAsync(code);
            if (existingEntity == null || existingEntity.Status != StatusConstant.Approved)
            {
                throw new Exception($"Branch with code '{code}' not found or not approved.");
            }

            var requester = _userService.GetCurrentUsername() ?? "system";
            if (string.IsNullOrWhiteSpace(requester))
            {
                throw new ArgumentException("Requester cannot be empty.");
            }

            // ===== Create update request =====
            var branchRequest = new BranchRequest
            {
                Action = RequestTypeConstant.Update,
                EntityId = existingEntity.Id,
                //EntityCode = code,
                Status = RequestStatusConstant.Unauthorised,
                RequestedData = JsonSerializer.Serialize(dto),
                //OriginalData = JsonSerializer.Serialize(new
                //{
                //    Name = existingEntity.Name,
                //    Description = existingEntity.Description ?? string.Empty,
                //    IsActive = existingEntity.IsActive
                //}),
                MakerId = requester
            };

            // ===== Update entity status =====
            existingEntity.Status = StatusConstant.Pending;
            await _branchRepository.UpdateAsync(existingEntity);

            await _branchRequestRepository.CreateAsync(branchRequest);
            return branchRequest.Id;
        }

        public async Task<long> CreateDeleteBranchRequestAsync(string code, DeleteBranchRequestDto request)
        {
            // ===== Validation =====
            var existingEntity = await _branchRepository.GetByCodeAsync(code);
            if (existingEntity == null || existingEntity.Status != StatusConstant.Approved)
            {
                throw new Exception($"Branch with code '{code}' not found or not approved.");
            }

            var requester = _userService.GetCurrentUsername() ?? "system";
            if (string.IsNullOrWhiteSpace(requester))
            {
                throw new ArgumentException("Requester cannot be empty.");
            }

            // ===== Create delete request =====
            var branchRequest = new BranchRequest
            {
                Action = RequestTypeConstant.Delete,
                EntityId = existingEntity.Id,
                //EntityCode = code,
                Status = RequestStatusConstant.Unauthorised,
                RequestedData = JsonSerializer.Serialize(request),
                //OriginalData = JsonSerializer.Serialize(new
                //{
                //    Name = existingEntity.Name,
                //    Description = existingEntity.Description ?? string.Empty,
                //    IsActive = existingEntity.IsActive
                //}),
                MakerId = requester
            };

            // ===== Update entity status =====
            existingEntity.Status = StatusConstant.Pending;
            await _branchRepository.UpdateAsync(existingEntity);

            await _branchRequestRepository.CreateAsync(branchRequest);
            return branchRequest.Id;
        }
        #endregion

        #region Pending Management
        public async Task<PagedResult<BranchPendingPagingDto>> GetPendingBranchesPagedAsync(GetBranchPagingRequest request)
        {
            return await _branchRequestRepository.GetPendingPagedAsync(request);
        }

        public async Task<BranchRequestDetailDto> GetPendingBranchByIdAsync(long requestId)
        {
            var request = await _branchRequestRepository.GetPendingByIdAsync(requestId);
            if (request == null)
            {
                throw new Exception($"Pending branch request with ID '{requestId}' not found.");
            }

            return new BranchRequestDetailDto
            {
                Id = request.Id,
                Action = request.Action,
                EntityId = request.EntityId,
                //EntityCode = request.EntityCode,
                Status = request.Status,
                CreatedBy = request.MakerId,
                CreatedDate = request.RequestedDate,
                CheckerId = request.CheckerId,
                ApproveDate = request.ApproveDate,
                Comments = request.Comments,
                RequestData = null, // Không thể deserialize vì không biết loại dữ liệu
                //OriginalData = !string.IsNullOrEmpty(request.OriginalData) 
                //    ? JsonSerializer.Deserialize<BranchDto>(request.OriginalData) 
                //    : null
            };
        }

        public async Task<BranchApprovalResultDto> ApprovePendingBranchRequestAsync(long requestId, string? comment = null)
        {
            // ===== Validation =====
            if (requestId <= 0)
            {
                throw new ArgumentException("RequestId must be greater than 0.", nameof(requestId));
            }

            var approver = _userService.GetCurrentUsername() ?? "system";
            if (string.IsNullOrWhiteSpace(approver))
            {
                throw new ArgumentException("Approver cannot be empty.");
            }

            // ===== Get request data =====
            var request = await _branchRequestRepository
                .FindAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
            {
                throw new Exception($"Pending branch request with ID '{requestId}' not found.");
            }

            // ===== Process approval with transaction =====
            await using var transaction = await _branchRequestRepository.BeginTransactionAsync();
            try
            {
                // ===== Process entity changes =====
                await ProcessBranchApproval(request);

                // ===== Update request status =====
                await UpdateRequestStatus(request, approver, comment);

                await transaction.CommitAsync();

                // ===== Return result =====
                return new BranchApprovalResultDto
                {
                    RequestId = request.Id,
                    RequestType = request.Action,
                    Status = RequestStatusConstant.Authorised,
                    ApprovedBy = approver,
                    ApprovedDate = DateTime.UtcNow,
                    Comment = comment ?? "Approved"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to approve branch request ID '{requestId}'.");
            }
        }

        public async Task<BranchApprovalResultDto> RejectPendingBranchRequestAsync(long requestId, string? reason = null)
        {
            // ===== Validation =====
            if (requestId <= 0)
            {
                throw new ArgumentException("RequestId must be greater than 0.", nameof(requestId));
            }

            var rejecter = _userService.GetCurrentUsername() ?? "system";
            if (string.IsNullOrWhiteSpace(rejecter))
            {
                throw new ArgumentException("Rejecter cannot be empty.");
            }

            // ===== Get request data =====
            var request = await _branchRequestRepository
                .FindAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
            {
                throw new Exception($"Pending branch request with ID '{requestId}' not found.");
            }

            // ===== Process rejection with transaction =====
            await using var transaction = await _branchRequestRepository.BeginTransactionAsync();
            try
            {
                // ===== Revert entity status if needed =====
                await RevertBranchStatusIfNeeded(request);

                // ===== Update request status =====
                await UpdateRejectedRequestStatus(request, rejecter, reason);

                await transaction.CommitAsync();

                // ===== Return result =====
                return new BranchApprovalResultDto
                {
                    RequestId = request.Id,
                    RequestType = request.Action,
                    Status = RequestStatusConstant.Rejected,
                    ApprovedBy = rejecter,
                    ApprovedDate = DateTime.UtcNow,
                    Comment = reason ?? "Rejected"
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to reject branch request ID '{requestId}'.");
            }
        }
        #endregion

        #region Private Methods
        private async Task ProcessBranchApproval(BranchRequest request)
        {
            switch (request.Action)
            {
                case RequestTypeConstant.Create:
                    await ProcessCreateBranch(request);
                    break;
                case RequestTypeConstant.Update:
                    await ProcessUpdateBranch(request);
                    break;
                case RequestTypeConstant.Delete:
                    await ProcessDeleteBranch(request);
                    break;
                default:
                    throw new ArgumentException($"Unsupported action: {request.Action}");
            }
        }

        private async Task ProcessCreateBranch(BranchRequest request)
        {
            var requestData = JsonSerializer.Deserialize<CreateBranchRequestDto>(request.RequestedData);
            if (requestData == null)
            {
                throw new Exception("Invalid request data for CREATE action.");
            }

            var branch = new Branch
            {
                Code = requestData.Code,
                Name = requestData.Name,
                Description = requestData.Description ?? string.Empty,
                Status = StatusConstant.Approved,
                IsActive = true
            };

            await _branchRepository.CreateAsync(branch);
        }

        private async Task ProcessUpdateBranch(BranchRequest request)
        {
            var requestData = JsonSerializer.Deserialize<UpdateBranchRequestDto>(request.RequestedData);
            if (requestData == null)
            {
                throw new Exception("Invalid request data for UPDATE action.");
            }

            var branch = await _branchRepository.GetByIdAsync(request.EntityId);
            if (branch == null)
            {
                throw new Exception($"Branch with ID '{request.EntityId}' not found.");
            }

            branch.Name = requestData.Name;
            branch.Description = requestData.Description ?? string.Empty;
            branch.Status = StatusConstant.Approved;

            await _branchRepository.UpdateAsync(branch);
        }

        private async Task ProcessDeleteBranch(BranchRequest request)
        {
            var branch = await _branchRepository.GetByIdAsync(request.EntityId);
            if (branch == null)
            {
                throw new Exception($"Branch with ID '{request.EntityId}' not found.");
            }

            branch.IsActive = false;
            branch.Status = StatusConstant.Deleted;

            await _branchRepository.UpdateAsync(branch);
        }

        private async Task RevertBranchStatusIfNeeded(BranchRequest request)
        {
            if ((request.Action == RequestTypeConstant.Update || request.Action == RequestTypeConstant.Delete) 
                && request.EntityId > 0)
            {
                var branch = await _branchRepository.GetByIdAsync(request.EntityId);
                if (branch != null)
                {
                    branch.Status = StatusConstant.Approved; // Revert to approved status
                    await _branchRepository.UpdateAsync(branch);
                }
            }
        }

        private async Task UpdateRequestStatus(BranchRequest request, string approver, string? comment)
        {
            request.Status = RequestStatusConstant.Authorised;
            request.CheckerId = approver;
            request.ApproveDate = DateTime.UtcNow;
            request.Comments = comment ?? "Approved";

            await _branchRequestRepository.UpdateAsync(request);
        }

        private async Task UpdateRejectedRequestStatus(BranchRequest request, string rejecter, string? reason)
        {
            request.Status = RequestStatusConstant.Rejected;
            request.CheckerId = rejecter;
            request.ApproveDate = DateTime.UtcNow;
            request.Comments = reason ?? "Rejected";

            await _branchRequestRepository.UpdateAsync(request);
        }
        #endregion
    }
}
