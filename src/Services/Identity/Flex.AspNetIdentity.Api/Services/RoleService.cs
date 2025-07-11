using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Flex.Infrastructure.EF;
using Flex.Shared.Constants.Common;

namespace Flex.AspNetIdentity.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IRoleRequestRepository _roleRequestRepository;

        public RoleService(ILogger<RoleService> logger, IRoleRequestRepository roleRequestRepository, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _logger = logger;
            _roleRequestRepository = roleRequestRepository;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        #region Query

        /// <summary>
        /// Trả về danh sách phân trang, gồm cả Entity đã duyệt và yêu cầu đang chờ.
        /// </summary>
        public async Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request)
        {
            // ===== Xử lý tham số phân trang & tìm kiếm =====
            var keyword = request?.Keyword?.Trim();
            var status = request?.Status?.ToUpper() ?? StatusConstant.Approved;
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== Truy vấn danh sách đã duyệt (APPROVED) =====
            var roleQuery = _roleManager.Roles
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    x => EF.Functions.Like(x.Code, $"%{keyword}%") ||
                         EF.Functions.Like(x.Description, $"%{keyword}%"))
                .AsNoTracking();

            if (status == RequestStatusConstant.Authorised)
            {
                // Chỉ lấy từ bảng chính
                var filteredRoleQuery = roleQuery.AsQueryable();
                var total = await filteredRoleQuery.CountAsync();
                var items = await filteredRoleQuery
                    .OrderBy(x => x.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(x => new RolePagingDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Code = x.Code,
                        IsActive = x.IsActive,
                        Description = x.Description,
                        Status = StatusConstant.Approved,
                        // Các trường khác nếu cần
                    })
                    .ToListAsync();
                return PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);
            }
            else if (status == RequestStatusConstant.Unauthorised)
            {
                // ===== Truy vấn các yêu cầu đang chờ duyệt hoặc nháp (PENDING hoặc DRAFT) =====
                var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery()
                    .Where(r => r.Status == RequestStatusConstant.Unauthorised)
                    .WhereIf(!string.IsNullOrEmpty(keyword),
                        r => EF.Functions.Like(r.Code, $"%{keyword}%") ||
                             EF.Functions.Like(r.Description, $"%{keyword}%"))
                    .AsNoTracking();

                // ===== Tạo danh sách các yêu cầu tạo mới Role (PENDING CREATE) =====
                var pendingCreatesQuery = proposedBranchQuery
                    .Where(r => r.Action == RequestTypeConstant.Create)
                    .Select(r => new RolePagingDto
                    {
                        Id = null,
                        Code = r.Code,
                        Name = r.Name,
                        IsActive = r.IsActive,
                        Description = r.Description,
                        Status = r.Status,
                        RequestType = r.Action,
                        RequestedBy = r.CreatedBy,
                        RequestedDate = r.CreatedDate,
                        ApprovedBy = null,
                        ApprovedDate = null
                    });


                // ===== SORT =====
                pendingCreatesQuery = pendingCreatesQuery
                    .OrderBy(dto => dto.Status == RequestStatusConstant.Draft ? 0 : dto.Status == RequestStatusConstant.Unauthorised ? 1 : 2)
                    .ThenByDescending(dto => (dto.Status == RequestStatusConstant.Draft || dto.Status == RequestStatusConstant.Unauthorised) ? dto.RequestedDate : null)
                    .ThenBy(dto => dto.Id);

                var total = await pendingCreatesQuery.CountAsync();
                var items = await pendingCreatesQuery
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);
            }

            return null;
        }

        /// <summary>
        /// Lấy thông tin chi tiết Role theo Id, kèm theo trạng thái yêu cầu đang chờ (nếu có).
        /// </summary>
        public async Task<RoleDto?> GetRoleByIdAsync(long id)
        {
            // 1. Ưu tiên tìm request nháp hoặc chờ duyệt theo EntityId
            var pendingRequest = await _roleRequestRepository.GetBranchCombinedQuery()
                .Where(r => r.EntityId == id && (r.Status == RequestStatusConstant.Unauthorised || r.Status == RequestStatusConstant.Draft))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (pendingRequest != null)
            {
                return new RoleDto
                {
                    Id = pendingRequest.EntityId,
                    Name = pendingRequest.Name,
                    Code = pendingRequest.Code,
                    IsActive = pendingRequest.IsActive,
                    Description = pendingRequest.Description,
                    HasPendingRequest = true,
                    PendingRequestId = pendingRequest.Id,
                    RequestType = pendingRequest.Action,
                    RequestedBy = pendingRequest.CreatedBy,
                    RequestedAt = pendingRequest.CreatedDate,
                    Status = pendingRequest.Status,
                };
            }

            // 2. Nếu không có request, tìm ở bảng chính
            var role = await _roleManager.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Code = role.Code,
                IsActive = role.IsActive,
                Description = role.Description,
                HasPendingRequest = pendingRequest != null,
                PendingRequestId = pendingRequest?.Id,
                RequestType = pendingRequest?.Action,
                RequestedBy = pendingRequest?.CreatedBy,
                RequestedAt = pendingRequest?.CreatedDate,
                Status = pendingRequest?.Status ?? StatusConstant.Approved,
            };
        }

        public async Task<RoleDto?> GetRoleByCodeAsync(string code)
        {
            // 1. Ưu tiên tìm request nháp hoặc chờ duyệt theo code
            var pendingRequest = await _roleRequestRepository.GetBranchCombinedQuery()
                .Where(r => r.Code == code && (r.Status == RequestStatusConstant.Unauthorised || r.Status == RequestStatusConstant.Draft))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (pendingRequest != null)
            {
                return new RoleDto
                {
                    Id = pendingRequest.EntityId,
                    Name = pendingRequest.Name,
                    Code = pendingRequest.Code,
                    IsActive = pendingRequest.IsActive,
                    Description = pendingRequest.Description,
                    HasPendingRequest = true,
                    PendingRequestId = pendingRequest.Id,
                    RequestType = pendingRequest.Action,
                    RequestedBy = pendingRequest.CreatedBy,
                    RequestedAt = pendingRequest.CreatedDate,
                    Status = pendingRequest.Status,
                };
            }

            // 2. Nếu không có request, tìm ở bảng chính
            var role = await _roleManager.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == code);

            if (role == null)
                return null;

            // Kiểm tra xem có pending request nào liên quan không (ví dụ update)
            var relatedPendingRequest = await _roleRequestRepository.GetBranchCombinedQuery()
                .Where(r => r.EntityId == role.Id && (r.Status == RequestStatusConstant.Unauthorised || r.Status == RequestStatusConstant.Draft))
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Code = role.Code,
                IsActive = role.IsActive,
                Description = role.Description,
                HasPendingRequest = relatedPendingRequest != null,
                PendingRequestId = relatedPendingRequest?.Id,
                RequestType = relatedPendingRequest?.Action,
                RequestedBy = relatedPendingRequest?.CreatedBy,
                RequestedAt = relatedPendingRequest?.CreatedDate,
                Status = relatedPendingRequest?.Status ?? StatusConstant.Approved,
            };
        }

        public async Task<IEnumerable<RoleChangeLogDto>> GetRoleChangeHistoryAsync(long roleId)
        {
            var requests = await _roleRequestRepository
                .FindAll() // giả định bạn có phương thức này hoặc dùng _dbContext.RoleRequests
                .Where(r => r.EntityId == roleId) // hoặc r.RoleId tùy tên cột
                .OrderByDescending(r => r.RequestedDate)
                .Select(r => new RoleChangeLogDto
                {
                    RequestId = r.Id,
                    RequestType = r.Action,
                    Status = r.Status,
                    RequestedBy = r.MakerId,
                    RequestedDate = r.RequestedDate,
                    ApprovedBy = r.CheckerId,
                    ApprovedDate = r.ApproveDate,
                    RejectReason = r.Comments,
                    Comment = r.Comments,
                    SnapshotJson = r.RequestedData // thường là JSON của Role
                })
                .ToListAsync();

            return requests;
        }
        public async Task<RoleRequestDto?> GetRoleRequestByIdAsync(long requestId)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                return null;

            var proposedData = string.IsNullOrEmpty(request.RequestedData)
                ? null
                : JsonSerializer.Deserialize<RoleDto>(request.RequestedData);

            return new RoleRequestDto
            {
                RequestId = request.Id,
                RoleId = request.EntityId, // hoặc request.RoleId, tùy DB
                RequestType = request.Action,
                Status = request.Status,
                ProposedData = proposedData
            };
        }
        //public async Task<string?> CompareRoleWithRequestAsync(long requestId)
        //{
        //    var request = await _roleRequestRepository
        //        .FindAll()
        //        .FirstOrDefaultAsync(r => r.Id == requestId);

        //    if (request == null || string.IsNullOrEmpty(request.RequestedData))
        //        return null;

        //    var proposed = JsonSerializer.Deserialize<RoleDto>(request.RequestedData);
        //    if (proposed == null)
        //        return null;

        //    var role = request.EntityId == null
        //        ? await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId)
        //        : null;

        //    var diffs = new List<FieldDiffDto>();

        //    // ===== So sánh các trường cơ bản =====
        //    if (role != null)
        //    {
        //        if (role.Name != proposed.Name)
        //            diffs.Add(new FieldDiffDto { Field = "Name", Original = role.Name, Proposed = proposed.Name });

        //        if (role.Description != proposed.Description)
        //            diffs.Add(new FieldDiffDto { Field = "Description", Original = role.Description, Proposed = proposed.Description });

        //        if (role.Code != proposed.Code)
        //            diffs.Add(new FieldDiffDto { Field = "Code", Original = role.Code, Proposed = proposed.Code });
        //    }
        //    else
        //    {
        //        // Tạo mới
        //        diffs.Add(new FieldDiffDto { Field = "Name", Original = null, Proposed = proposed.Name });
        //        diffs.Add(new FieldDiffDto { Field = "Description", Original = null, Proposed = proposed.Description });
        //        diffs.Add(new FieldDiffDto { Field = "Code", Original = null, Proposed = proposed.Code });
        //    }

        //    // ===== So sánh claims =====
        //    var currentClaims = role != null ? await _roleManager.GetClaimsAsync(role) : new List<Claim>();
        //    var proposedClaims = proposed.Claims ?? new List<ClaimDto>();

        //    var removedClaims = currentClaims
        //        .Where(c => !proposedClaims.Any(p => p.Type == c.Type && p.Value == c.Value))
        //        .Select(c => new FieldDiffDto
        //        {
        //            Field = "Claim",
        //            Original = $"{c.Type}:{c.Value}",
        //            Proposed = null
        //        });

        //    var addedClaims = proposedClaims
        //        .Where(p => !currentClaims.Any(c => c.Type == p.Type && c.Value == p.Value))
        //        .Select(p => new FieldDiffDto
        //        {
        //            Field = "Claim",
        //            Original = null,
        //            Proposed = $"{p.Type}:{p.Value}"
        //        });

        //    diffs.AddRange(removedClaims);
        //    diffs.AddRange(addedClaims);

        //    return JsonSerializer.Serialize(diffs, new JsonSerializerOptions
        //    {
        //        WriteIndented = true
        //    });
        //}
        #endregion

        #region Command 
        #endregion

        #region Flow 
        #endregion

        #region Domain Mapping 
        #endregion

        #region Command
        public async Task<long> CreateAddRoleRequestAsync(CreateRoleDto dto, string requestedBy)
        {
            _logger.LogInformation($"[CreateAddRoleRequestAsync] dto.Status: {dto.Status}, dto.Description: {dto.Description}");
            var requestedJson = JsonSerializer.Serialize(dto);
            var request = new RoleRequest
            {
                Action = RequestTypeConstant.Create,
                Status = (dto.Status != null && dto.Status.Equals("Draft", StringComparison.OrdinalIgnoreCase))
                    ? RequestStatusConstant.Draft
                    : RequestStatusConstant.Unauthorised,
                EntityId = 0,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                RequestedData = requestedJson
            };
            await _roleRequestRepository.CreateAsync(request);
            return request.Id;
        }
        public async Task<long> CreateUpdateRoleRequestAsync(long roleId, UpdateRoleDto dto, string requestedBy)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
                throw new Exception("Role not found");
            var existingRequest = await _roleRequestRepository.FindAll()
                .AnyAsync(r => r.EntityId == roleId &&
                               r.Status == RequestStatusConstant.Unauthorised &&
                               r.Action == RequestTypeConstant.Update);
            if (existingRequest)
                throw new Exception("A pending update request already exists for this role.");
            var requestedJson = JsonSerializer.Serialize(dto);
            var request = new RoleRequest
            {
                Action = RequestTypeConstant.Update,
                Status = RequestStatusConstant.Unauthorised,
                EntityId = roleId,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                RequestedData = requestedJson
            };
            await _roleRequestRepository.CreateAsync(request);
            return request.Id;
        }
        public async Task<long> CreateDeleteRoleRequestAsync(long roleId, string requestedBy)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role == null)
                throw new Exception("Role not found.");
            var hasPendingDelete = await _roleRequestRepository.FindAll()
                .AnyAsync(r => r.EntityId == roleId &&
                               r.Status == RequestStatusConstant.Unauthorised &&
                               r.Action == RequestTypeConstant.Delete);
            if (hasPendingDelete)
                throw new Exception("A pending delete request already exists for this role.");
            var claims = await _roleManager.GetClaimsAsync(role);
            var currentSnapshot = new RoleDto
            {
                Id = role.Id,
                Code = role.Code,
                Name = role.Name,
                Description = role.Description,
            };
            var request = new RoleRequest
            {
                Action = RequestTypeConstant.Delete,
                Status = RequestStatusConstant.Unauthorised,
                EntityId = roleId,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                RequestedData = JsonSerializer.Serialize(currentSnapshot)
            };
            await _roleRequestRepository.CreateAsync(request);
            return request.Id;
        }
        public async Task ApproveRoleRequestAsync(long requestId, string? comment = null)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
                throw new Exception("Pending role request not found.");

            var approvedBy = "system";

            // Xử lý theo loại yêu cầu
            if (request.Action == RequestTypeConstant.Create)
            {
                var dto = JsonSerializer.Deserialize<CreateRoleDto>(request.RequestedData);
                if (dto == null) throw new Exception("Invalid request data");

                var newRole = new Role("", "");

                await _roleManager.CreateAsync(newRole);

                if (dto.Claims != null)
                {
                    //foreach (var claim in dto.Claims)
                    //{
                    //    await _roleManager.AddClaimAsync(newRole, new System.Security.Claims.Claim(claim.Type, claim.Value));
                    //}
                }

                request.EntityId = newRole.Id;
            }
            else if (request.Action == RequestTypeConstant.Update)
            {
                var dto = JsonSerializer.Deserialize<UpdateRoleDto>(request.RequestedData);
                if (dto == null) throw new Exception("Invalid request data");

                var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId);
                if (role == null) throw new Exception("Original role not found");

                //role.Name = dto.Name;
                //role.Description = dto.Description;
                //role.LastModifiedBy = approvedBy;
                //role.LastModifiedDate = DateTime.UtcNow;

                await _roleManager.UpdateAsync(role);

                //if (dto.Claims != null)
                //{
                //    var existingClaims = await _roleManager.GetClaimsAsync(role);

                //    foreach (var c in existingClaims)
                //        await _roleManager.RemoveClaimAsync(role, c);

                //    //foreach (var claim in dto.Claims)
                //    //    await _roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, claim.Value));
                //}
            }
            else if (request.Action == RequestTypeConstant.Delete)
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId);
                if (role == null) throw new Exception("Role not found");

                await _roleManager.DeleteAsync(role);
            }

            // Cập nhật trạng thái yêu cầu
            //request.Status = RequestStatusConstant.Approved;
            //request.ApprovedBy = approvedBy;
            //request.ApprovedDate = DateTime.UtcNow;
            //request.Comment = comment;

            await _roleRequestRepository.UpdateAsync(request);
        }
        public async Task RejectRoleRequestAsync(long requestId, string reason)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
                throw new Exception("Pending request not found.");

            request.Status = RequestStatusConstant.Rejected;
            //request.ApprovedBy = _currentUser?.UserName ?? "system";
            //request.ApprovedDate = DateTime.UtcNow;
            //request.RejectReason = reason;

            await _roleRequestRepository.UpdateAsync(request);
        }
        public async Task CancelRoleRequestAsync(long requestId, string currentUser)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised || r.Status == RequestStatusConstant.Draft);

            if (request == null)
                throw new Exception("Pending draft request not found or already processed.");

            if (request.MakerId != currentUser)
                throw new Exception("You can only cancel your own draft request.");

            request.Status = RequestStatusConstant.Cancelled;
            request.ApproveDate = DateTime.UtcNow;

            await _roleRequestRepository.UpdateAsync(request);
        }
        #endregion
        public async Task AddClaimsAsync(long roleId, IEnumerable<ClaimDto> claims)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId)
                       ?? throw new Exception("Role not found");

            foreach (var claim in claims)
            {
                await _roleManager.AddClaimAsync(role, new Claim(claim.Type, claim.Value));
            }
        }
        public Task<long> CreateAsync(CreateRoleDto dto)
        {
            throw new NotImplementedException();
        }
        public Task<long> CreateFromApprovedRequestAsync(string requestedDataJson)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<ClaimDto>> GetClaimsAsync(long roleId)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId)
                       ?? throw new Exception("Role not found");

            var claims = await _roleManager.GetClaimsAsync(role);
            return claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value });
        }

        public async Task RemoveClaimAsync(long roleId, ClaimDto claim)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == roleId)
                       ?? throw new Exception("Role not found");
            await _roleManager.RemoveClaimAsync(role, new Claim(claim.Type, claim.Value));
        }
        public Task UpdateAsync(long roleId, UpdateRoleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<RolePagingDto>> GetRolePagedAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<RoleImpactDto>> GetRoleRequestImpactAsync(long requestId)
        {
            throw new NotImplementedException();
        }

        public Task<string?> CompareRoleWithRequestAsync(long requestId)
        {
            throw new NotImplementedException();
        }

        public async Task<RoleRequestDto?> GetDraftCreateRequestByCodeAsync(string code, string currentUser)
        {
            // 1. Tìm roleId theo code
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Code == code);
            if (role == null) return null;

            // 2. Tìm bản nháp theo roleId
            var draft = await _roleRequestRepository.FindAll()
                .Where(r => r.EntityId == role.Id && r.Status == RequestStatusConstant.Draft && r.Action == RequestTypeConstant.Create && r.MakerId == currentUser)
                .OrderByDescending(r => r.RequestedDate)
                .FirstOrDefaultAsync();
            if (draft == null) return null;
            return new RoleRequestDto
            {
                RequestId = draft.Id,
                RoleId = draft.EntityId,
                RequestType = draft.Action,
                Status = draft.Status,
                RequestedBy = draft.MakerId,
                RequestedDate = draft.RequestedDate,
                ApprovedBy = draft.CheckerId,
                ApprovedDate = draft.ApproveDate,
                ProposedData = string.IsNullOrEmpty(draft.RequestedData) ? null : JsonSerializer.Deserialize<RoleDto>(draft.RequestedData)
            };
        }

        public async Task<RoleRequestDto?> GetDraftByRoleAsync(long roleId)
        {
            var draft = await _roleRequestRepository.FindAll()
                .Where(r => r.EntityId == roleId && (r.Status == RequestStatusConstant.Unauthorised || r.Status == RequestStatusConstant.Draft))
                .OrderByDescending(r => r.RequestedDate)
                .FirstOrDefaultAsync();
            if (draft == null) return null;
            return new RoleRequestDto
            {
                RequestId = draft.Id,
                RoleId = draft.EntityId,
                RequestType = draft.Action,
                Status = draft.Status,
                RequestedBy = draft.MakerId,
                RequestedDate = draft.RequestedDate,
                ApprovedBy = draft.CheckerId,
                ApprovedDate = draft.ApproveDate,
                ProposedData = string.IsNullOrEmpty(draft.RequestedData) ? null : JsonSerializer.Deserialize<RoleDto>(draft.RequestedData)
            };
        }

        // ===== API MỚI =====
        
        /// <summary>
        /// Lấy danh sách yêu cầu chờ duyệt (Pending/Draft)
        /// </summary>
        public async Task<PagedResult<RoleRequestDto>> GetPendingRequestsAsync(PendingRequestsPagingRequest request)
        {
            var keyword = request?.Keyword?.Trim();
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            var query = _roleRequestRepository.GetBranchCombinedQuery()
                .Where(r => r.Status == RequestStatusConstant.Unauthorised || r.Status == RequestStatusConstant.Draft)
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    r => EF.Functions.Like(r.Code, $"%{keyword}%") ||
                         EF.Functions.Like(r.Name, $"%{keyword}%") ||
                         EF.Functions.Like(r.Description, $"%{keyword}%"))
                .AsNoTracking();

            // Filter theo RequestType
            if (!string.IsNullOrEmpty(request?.RequestType) && request.RequestType.ToUpper() != "ALL")
            {
                query = query.Where(r => r.Action == request.RequestType);
            }

            // Filter theo Status
            if (!string.IsNullOrEmpty(request?.Status) && request.Status.ToUpper() != "ALL")
            {
                query = query.Where(r => r.Status == request.Status);
            }

            // Sort
            query = query.OrderByDescending(r => r.CreatedDate);

            var total = await query.CountAsync();
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new RoleRequestDto
                {
                    RequestId = r.Id,
                    RoleId = r.EntityId,
                    RequestType = r.Action,
                    Status = r.Status,
                    RequestedBy = r.CreatedBy,
                    RequestedDate = r.CreatedDate,
                    // ProposedData = null // Không có dữ liệu đề xuất ở view này
                })
                .ToListAsync();

            return PagedResult<RoleRequestDto>.Create(pageIndex, pageSize, total, items);
        }

        /// <summary>
        /// So sánh bản chính và bản nháp
        /// </summary>
        public async Task<RoleComparisonDto?> GetRoleComparisonAsync(long requestId)
        {
            // Truy vấn từ entity gốc để lấy RequestedData
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                return null;

            var proposedData = string.IsNullOrEmpty(request.RequestedData)
                ? null
                : JsonSerializer.Deserialize<RoleDto>(request.RequestedData);

            if (proposedData == null)
                return null;

            var comparison = new RoleComparisonDto
            {
                RequestId = request.Id,
                RequestType = request.Action,
                RequestedBy = request.MakerId,
                RequestedDate = request.RequestedDate,
                ProposedVersion = proposedData,
                Changes = new List<FieldDiffDto>()
            };

            // Lấy bản chính hiện tại (nếu có)
            RoleDto? currentVersion = null;
            if (request.Action == RequestTypeConstant.Update || request.Action == RequestTypeConstant.Delete)
            {
                var currentRole = await _roleManager.Roles
                    .Where(r => r.Id == request.EntityId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (currentRole != null)
                {
                    currentVersion = new RoleDto
                    {
                        Id = currentRole.Id,
                        Name = currentRole.Name,
                        Code = currentRole.Code,
                        Description = currentRole.Description,
                        IsActive = currentRole.IsActive
                    };
                    comparison.CurrentVersion = currentVersion;
                }
            }

            // So sánh các trường
            if (request.Action == RequestTypeConstant.Create)
            {
                // Tạo mới - tất cả trường đều là "Added"
                comparison.Changes.Add(new FieldDiffDto
                {
                    FieldName = "Name",
                    CurrentValue = null,
                    ProposedValue = proposedData.Name,
                    ChangeType = "Added"
                });
                comparison.Changes.Add(new FieldDiffDto
                {
                    FieldName = "Code",
                    CurrentValue = null,
                    ProposedValue = proposedData.Code,
                    ChangeType = "Added"
                });
                comparison.Changes.Add(new FieldDiffDto
                {
                    FieldName = "Description",
                    CurrentValue = null,
                    ProposedValue = proposedData.Description,
                    ChangeType = "Added"
                });
            }
            else if (request.Action == RequestTypeConstant.Update && currentVersion != null)
            {
                // Cập nhật - so sánh từng trường
                if (currentVersion.Name != proposedData.Name)
                {
                    comparison.Changes.Add(new FieldDiffDto
                    {
                        FieldName = "Name",
                        CurrentValue = currentVersion.Name,
                        ProposedValue = proposedData.Name,
                        ChangeType = "Modified"
                    });
                }

                if (currentVersion.Code != proposedData.Code)
                {
                    comparison.Changes.Add(new FieldDiffDto
                    {
                        FieldName = "Code",
                        CurrentValue = currentVersion.Code,
                        ProposedValue = proposedData.Code,
                        ChangeType = "Modified"
                    });
                }

                if (currentVersion.Description != proposedData.Description)
                {
                    comparison.Changes.Add(new FieldDiffDto
                    {
                        FieldName = "Description",
                        CurrentValue = currentVersion.Description,
                        ProposedValue = proposedData.Description,
                        ChangeType = "Modified"
                    });
                }
            }
            else if (request.Action == RequestTypeConstant.Delete && currentVersion != null)
            {
                // Xóa - tất cả trường đều là "Removed"
                comparison.Changes.Add(new FieldDiffDto
                {
                    FieldName = "Name",
                    CurrentValue = currentVersion.Name,
                    ProposedValue = null,
                    ChangeType = "Removed"
                });
                comparison.Changes.Add(new FieldDiffDto
                {
                    FieldName = "Code",
                    CurrentValue = currentVersion.Code,
                    ProposedValue = null,
                    ChangeType = "Removed"
                });
                comparison.Changes.Add(new FieldDiffDto
                {
                    FieldName = "Description",
                    CurrentValue = currentVersion.Description,
                    ProposedValue = null,
                    ChangeType = "Removed"
                });
            }

            return comparison;
        }
    }
}
