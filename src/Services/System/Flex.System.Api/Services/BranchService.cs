using Flex.Infrastructure.EF;
using Flex.Shared.DTOs.Common;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Flex.System.Api.Entities;
using Flex.System.Api.Models;
using Flex.System.Api.Models.Branch;
using Flex.System.Api.Persistence;
using Flex.System.Api.Repositories.Interfaces;
using Flex.System.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        public async Task<PagedResult<BranchApprovedItemDto>> GetApprovedBranchesPagedAsync(GetBranchPagingRequest request)
        {
            // ===== Process request parameters =====
            var keyword = request?.Keyword?.Trim().ToLower();
            var status = request?.IsActive?.Trim().ToUpper() == "Y" ? true : false;
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== Build query =====
            var roleQuery = _branchRepository.FindAll()
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    x => EF.Functions.Like(x.Code.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(x.Description.ToLower(), $"%{keyword}%"))
                .WhereIf(!string.IsNullOrEmpty(request.IsActive), x => x.IsActive == status);

            // ===== Execute query =====
            var total = await roleQuery.CountAsync();
            var items = await roleQuery
                .OrderBy(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new BranchApprovedItemDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    Status = x.Status,
                    BranchType = x.BranchType,
                })
                .ToListAsync();

            // ===== Return result =====
            return PagedResult<BranchApprovedItemDto>.Create(pageIndex, pageSize, total, items);
        }

        public async Task<BranchDetailDto> GetApprovedBranchByCodeAsync(string code)
        {
            var entity = await _branchRepository.GetByCodeAsync(code);
            if (entity == null || entity.Status != RequestStatusConstant.Authorised)
            {
                throw new Exception($"Branch with code '{code}' not found or not approved.");
            }

            return new BranchDetailDto
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Description = entity.Description ?? string.Empty,
                Status = entity.Status,
                IsActive = entity.IsActive,
                BranchType = entity.BranchType
            };
        }

        public async Task<List<BranchChangeHistoryDto>> GetApprovedBranchChangeHistoryAsync(string code)
        {
            // ===== Find branch by code =====
            var branch = await _branchRepository.GetByCodeAsync(code);
            if (branch == null)
            {
                throw new Exception($"Branch with code '{code}' not exists.");
            }

            // ===== Get branch requests by entity Id =====
            var branchId = branch.Id;

            var requests = await _branchRequestRepository
                .FindAll()
                .Where(r => r.EntityId == branchId)
                .OrderByDescending(r => r.RequestedDate)
                .Select(r => new
                {
                    r.Id,
                    r.Action,
                    r.MakerId,
                    r.RequestedDate,
                    r.CheckerId,
                    r.ApproveDate,
                    r.Comments
                })
                .AsNoTracking()
                .ToListAsync();

            var historyItems = requests.Select(r => new BranchChangeHistoryDto
            {
                Id = r.Id,
                Operation = r.Action,
                RequestedBy = r.MakerId,
                ApprovedBy = r.CheckerId,
                RequestedDate = r.RequestedDate,
                ApprovedDate = r.ApproveDate,
                Comments = r.Comments
            }).ToList();

            return historyItems;
        }

        public async Task<List<OptionDto>> GetBranchesForFilterAsync()
        {
            var branches = await _branchRepository.FindAll()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new OptionDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .AsNoTracking()
                .ToListAsync();

            return branches;
        }

        public async Task<Dictionary<string, BranchDetailDto>> GetBranchesByCodesAsync(IEnumerable<string> codes)
        {
            if (codes == null || !codes.Any())
            {
                return new Dictionary<string, BranchDetailDto>();
            }

            var codeList = codes.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct().ToList();
            if (!codeList.Any())
            {
                return new Dictionary<string, BranchDetailDto>();
            }

            var branches = await _branchRepository.FindAll()
                .Where(x => codeList.Contains(x.Code) && 
                           x.Status == RequestStatusConstant.Authorised && 
                           x.IsActive)
                .Select(x => new BranchDetailDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description ?? string.Empty,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    BranchType = x.BranchType
                })
                .AsNoTracking()
                .ToListAsync();

            return branches.ToDictionary(x => x.Code, x => x);
        }

        public async Task<Dictionary<long, BranchDetailDto>> GetBranchesByIdsAsync(IEnumerable<long> ids)
        {
            if (ids == null || !ids.Any())
            {
                return new Dictionary<long, BranchDetailDto>();
            }

            var idList = ids.Where(id => id > 0).Distinct().ToList();
            if (!idList.Any())
            {
                return new Dictionary<long, BranchDetailDto>();
            }

            var branches = await _branchRepository.FindAll()
                .Where(x => idList.Contains(x.Id))
                .Select(x => new BranchDetailDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description ?? string.Empty,
                    Status = x.Status,
                    IsActive = x.IsActive,
                    BranchType = x.BranchType
                })
                .AsNoTracking()
                .ToListAsync();

            return branches.ToDictionary(x => x.Id, x => x);
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
                .CountAsync(v => v.Status == RequestStatusConstant.Unauthorised && v.Code == request.Code) > 0;
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
                Comments = "Yêu cầu thêm mới chi nhánh.",
                RequestedData = JsonSerializer.Serialize(request),
            };

            await _branchRequestRepository.CreateAsync(branchRequest);
            return branchRequest.Id;
        }

        public async Task<long> CreateUpdateBranchRequestAsync(string code, UpdateBranchRequestDto dto)
        {
            // ===== Validation =====
            var existingEntity = await _branchRepository.GetByCodeAsync(code);
            if (existingEntity == null || existingEntity.Status != RequestStatusConstant.Authorised)
            {
                throw new Exception($"Branch with code '{code}' not found or not approved.");
            }

            var requester = _userService.GetCurrentUsername() ?? "system";
            if (string.IsNullOrWhiteSpace(requester))
            {
                throw new ArgumentException("Requester cannot be empty.");
            }

            // ===== Create update request =====
            // ===== Check duplicate pending request via view =====
            var hasPending = await _dbContext.ProposedBranches
                .AsNoTracking()
                .CountAsync(v => v.Status == RequestStatusConstant.Unauthorised && v.EntityId == existingEntity.Id) > 0;
            if (hasPending)
            {
                throw new Exception($"Pending branch update request already exists for code '{code}'.");
            }

            var branchRequest = new BranchRequest
            {
                Action = RequestTypeConstant.Update,
                EntityId = existingEntity.Id,
                //EntityCode = code,
                Status = RequestStatusConstant.Unauthorised,
                RequestedData = JsonSerializer.Serialize(new
                {
                    Name = dto.Name?.Trim(),
                    Description = dto.Description?.Trim(),
                    BranchType = dto.BranchType,
                    IsActive = dto.IsActive
                }),
                //OriginalData = JsonSerializer.Serialize(new
                //{
                //    Name = existingEntity.Name,
                //    Description = existingEntity.Description ?? string.Empty,
                //    IsActive = existingEntity.IsActive
                //}),
                MakerId = requester
            };

            // ===== Update entity status =====
            existingEntity.Status = RequestStatusConstant.Unauthorised;
            await _branchRepository.UpdateAsync(existingEntity);

            await _branchRequestRepository.CreateAsync(branchRequest);
            return branchRequest.Id;
        }

        public async Task<long> CreateDeleteBranchRequestAsync(string code, DeleteBranchRequestDto request)
        {
            // ===== Validation =====
            var existingEntity = await _branchRepository.GetByCodeAsync(code);
            if (existingEntity == null || existingEntity.Status != RequestStatusConstant.Authorised)
            {
                throw new Exception($"Branch with code '{code}' not found or not approved.");
            }

            // ===== Guard: prevent duplicate logical deletion =====
            if (!existingEntity.IsActive)
            {
                throw new Exception($"Branch with code '{code}' is already inactive.");
            }

            var requester = _userService.GetCurrentUsername() ?? "system";
            if (string.IsNullOrWhiteSpace(requester))
            {
                throw new ArgumentException("Requester cannot be empty.");
            }

            // ===== Create delete request =====
            // ===== Check duplicate pending request via view =====
            var hasPending = await _dbContext.ProposedBranches
                .AsNoTracking()
                .CountAsync(v => v.Status == RequestStatusConstant.Unauthorised && v.EntityId == existingEntity.Id) > 0;
            if (hasPending)
            {
                throw new Exception($"Pending branch delete request already exists for code '{code}'.");
            }

            // ===== Build snapshot of current branch for audit/review (align with Role flow) =====
            var currentSnapshot = new BranchDto
            {
                Id = existingEntity.Id,
                Code = existingEntity.Code,
                Name = existingEntity.Name,
                Description = existingEntity.Description ?? string.Empty,
                IsActive = existingEntity.IsActive,
                Status = existingEntity.Status
            };

            var branchRequest = new BranchRequest
            {
                Action = RequestTypeConstant.Delete,
                EntityId = existingEntity.Id,
                Status = RequestStatusConstant.Unauthorised,
                RequestedData = JsonSerializer.Serialize(currentSnapshot),
                MakerId = requester,
                RequestedDate = DateTime.UtcNow,
                Comments = string.IsNullOrWhiteSpace(request?.Reason) ? "Yêu cầu xóa chi nhánh." : request.Reason
            };

            // ===== Transaction: create request + update entity status =====
            await using var transaction = await _branchRequestRepository.BeginTransactionAsync();
            try
            {
                await _branchRequestRepository.CreateAsync(branchRequest);

                existingEntity.Status = RequestStatusConstant.Unauthorised;
                await _branchRepository.UpdateAsync(existingEntity);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to create delete branch request.");
            }

            return branchRequest.Id;
        }
        #endregion

        #region Pending Management
        public async Task<PagedResult<BranchPendingPagingDto>> GetPendingBranchesPagedAsync(GetBranchPagingRequest request)
        {
            // ===== Process request parameters =====
            var keyword = request?.Keyword?.Trim().ToLower();
            var requestType = request?.Type?.Trim().ToUpper();
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== Build query =====
            var proposedBranchQuery = _branchRepository.GetBranchCombinedQuery()
                .Where(r => r.Status == RequestStatusConstant.Unauthorised)
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    r => EF.Functions.Like(r.Code.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(r.Description.ToLower(), $"%{keyword}%"))
                .WhereIf(!string.IsNullOrEmpty(requestType) && requestType != RequestTypeConstant.All,
                    r => r.Action == requestType)
                .AsNoTracking();

            var pendingQuery = proposedBranchQuery
                .Select(r => new BranchPendingPagingDto
                {
                    Code = r.Code,
                    Name = r.Name,
                    Description = r.Description,
                    RequestType = r.Action,
                    CreatedBy = r.CreatedBy,
                    CreatedDate = r.CreatedDate,
                    Id = r.Id ?? 0
                });

            // ===== Execute query =====
            var total = await pendingQuery.CountAsync();
            var items = await pendingQuery
                .OrderByDescending(dto => dto.CreatedDate)
                .ThenBy(dto => dto.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ===== Return result =====
            return PagedResult<BranchPendingPagingDto>.Create(pageIndex, pageSize, total, items);
        }

        public async Task<BranchRequestDetailDto> GetPendingBranchByIdAsync(long requestId)
        {
            var request = await _branchRequestRepository.GetPendingByIdAsync(requestId);
            if (request == null)
            {
                throw new Exception($"Pending branch request with ID '{requestId}' not found.");
            }

            var result = new BranchRequestDetailDto
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
                RequestData = null, // Will populate below for supported actions
                //OriginalData = !string.IsNullOrEmpty(request.OriginalData) 
                //    ? JsonSerializer.Deserialize<BranchDto>(request.OriginalData) 
                //    : null
            };

            // ===== Populate request data for DELETE (snapshot of current entity) =====
            if (request.Action == RequestTypeConstant.Delete && !string.IsNullOrEmpty(request.RequestedData))
            {
                try
                {
                    // RequestedData was stored as a BranchDetailDto snapshot; deserialize into BranchDto for display
                    var snapshot = JsonSerializer.Deserialize<BranchDto>(request.RequestedData);
                    if (snapshot != null)
                    {
                        result.RequestData = snapshot;
                    }
                }
                catch
                {
                    // Ignore deserialization issues; keep RequestData = null
                }
            }

            return result;
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

                // ===== Default approval comment by action (optional) =====
                switch (request.Action)
                {
                    case RequestTypeConstant.Create:
                        comment ??= "Yêu cầu thêm mới chi nhánh.";
                        break;
                    case RequestTypeConstant.Update:
                        comment ??= "Yêu cầu cập nhật chi nhánh.";
                        break;
                    case RequestTypeConstant.Delete:
                        comment ??= "Yêu cầu hủy kích hoạt chi nhánh.";
                        break;
                }

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
                Status = RequestStatusConstant.Authorised,
                BranchType = requestData.BranchType,
                IsActive = true
            };

            var createdId = await _branchRepository.CreateAsync(branch);
            request.EntityId = createdId;
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

            branch.Name = requestData.Name?.Trim() ?? branch.Name;
            branch.Description = requestData.Description?.Trim() ?? string.Empty;
            branch.BranchType = requestData.BranchType;
            branch.IsActive = requestData.IsActive;
            branch.Status = RequestStatusConstant.Authorised;

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
            branch.Status = RequestStatusConstant.Authorised;

            await _branchRepository.DeleteAsync(branch);
        }

        private async Task RevertBranchStatusIfNeeded(BranchRequest request)
        {
            if ((request.Action == RequestTypeConstant.Update || request.Action == RequestTypeConstant.Delete) 
                && request.EntityId > 0)
            {
                var branch = await _branchRepository.GetByIdAsync(request.EntityId);
                if (branch != null)
                {
                    branch.Status = RequestStatusConstant.Authorised; // Revert to approved status
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
