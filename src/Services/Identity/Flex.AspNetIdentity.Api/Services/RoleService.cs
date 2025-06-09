using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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
        //public async Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request)
        //{
        //    var keyword = request?.Keyword?.Trim().ToLower();
        //    int pageIndex = Math.Max(1, request.PageIndex ?? 1);
        //    int pageSize = Math.Max(1, request.PageSize ?? 10);

        //    // ========== QUERY ==========
        //    var roleQuery = _roleManager.Roles
        //        .WhereIf(!string.IsNullOrEmpty(keyword), x => x.Code.ToLower().Contains(keyword) || x.Description.ToLower().Contains(keyword))
        //        .AsNoTracking();
        //    var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery()
        //        .Where(r => r.Status == RequestStatusConstant.Unauthorised.ToString())
        //        .WhereIf(!string.IsNullOrEmpty(keyword), r => r.Code.ToLower().Contains(keyword) || r.Description.ToLower().Contains(keyword))
        //        .AsNoTracking();

        //    // ========== PENDING CREATE ==========
        //    var pendingCreates = proposedBranchQuery
        //        .Where(r => r.Action == RequestTypeConstant.Create)
        //        .Select(r => new RolePagingDto
        //        {
        //            Id = null,
        //            Name = r.Name,
        //            Code = r.Code,
        //            IsActive = r.IsActive,
        //            Description = r.Description,
        //            Status = StatusConstant.Pending.ToString(),
        //            RequestType = RequestTypeConstant.Create.ToString()
        //        });

        //    // ========== ROLES ==========
        //    var rolesWithOverlay = roleQuery
        //        .GroupJoin(
        //            proposedBranchQuery,
        //            role => role.Id,
        //            req => req.EntityId,
        //            (role, reqs) => new { role, req = reqs.FirstOrDefault() })
        //        .Select(x => new RolePagingDto
        //        {
        //            Id = x.role.Id,
        //            Name = x.role.Name,
        //            Code = x.role.Code,
        //            IsActive = x.role.IsActive,
        //            Description = x.role.Description,
        //            Status = x.req == null ? StatusConstant.Approved.ToString() : StatusConstant.Pending.ToString(),
        //            RequestType = x.req == null ? null : x.req.Action
        //        });

        //    // ========== UNION && ORDER ==========
        //    var combined = rolesWithOverlay.Union(pendingCreates)
        //        .OrderBy(dto => dto.Status == StatusConstant.Pending.ToString() ? 0 : 1)
        //        .ThenBy(dto => dto.Id);

        //    // ========== PAGINATION ==========
        //    var total = await combined.CountAsync();
        //    var items = await combined
        //                    .Skip((pageIndex - 1) * pageSize)
        //                    .Take(pageSize)
        //                    .ToListAsync();

        //    var resp = PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);

        //    return resp;
        //}


        public async Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request)
        {
            var keyword = request?.Keyword?.Trim();
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== TRUY VẤN ROLE GỐC =====
            var roleQuery = _roleManager.Roles
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    x => EF.Functions.Like(x.Code, $"%{keyword}%") ||
                         EF.Functions.Like(x.Description, $"%{keyword}%"))
                .AsNoTracking();

            // ===== TRUY VẤN REQUEST PENDING =====
            var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery()
                .Where(r => r.Status == RequestStatusConstant.Unauthorised)
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    r => EF.Functions.Like(r.Code, $"%{keyword}%") ||
                         EF.Functions.Like(r.Description, $"%{keyword}%"))
                .AsNoTracking();

            // ===== DANH SÁCH TẠO MỚI (PENDING CREATE) =====
            var pendingCreatesQuery = proposedBranchQuery
                .Where(r => r.Action == RequestTypeConstant.Create)
                .Select(r => new RolePagingDto
                {
                    Id = null,
                    Name = r.Name,
                    Code = r.Code,
                    IsActive = r.IsActive,
                    Description = r.Description,
                    Status = "PENDING", // literal string
                    RequestType = "CREATE" // literal string
                });

            // ===== GHÉP ROLE VỚI REQUEST (UPDATE/DELETE PENDING) =====
            var rolesWithOverlayQuery = roleQuery
                .GroupJoin(proposedBranchQuery,
                    role => role.Id,
                    req => req.EntityId,
                    (role, reqs) => new { role, req = reqs.FirstOrDefault() })
                .Select(x => new RolePagingDto
                {
                    Id = x.role.Id,
                    Name = x.role.Name,
                    Code = x.role.Code,
                    IsActive = x.role.IsActive,
                    Description = x.role.Description,
                    Status = (x.req == null ? "APPROVED" : "PENDING"), // dùng literal string
                    RequestType = (x.req == null ? null : x.req.Action + "") // ép EF không bind parameter
                });

            // ===== GHÉP & PHÂN TRANG =====
            var combinedQuery = rolesWithOverlayQuery.Union(pendingCreatesQuery)
                .OrderBy(dto => dto.Status == "PENDING" ? 0 : 1)
                .ThenBy(dto => dto.Id);

            var total = await combinedQuery.CountAsync();
            var items = await combinedQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);
        }


        //public async Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request)
        //{
        //    var keyword = request?.Keyword?.Trim();
        //    int pageIndex = Math.Max(1, request.PageIndex ?? 1);
        //    int pageSize = Math.Max(1, request.PageSize ?? 10);

        //    // Truy vấn role đã có
        //    var roleQuery = _roleManager.Roles
        //        .WhereIf(!string.IsNullOrEmpty(keyword), x => x.Code.ToLower().Contains(keyword) || x.Description.ToLower().Contains(keyword))
        //        .AsNoTracking();

        //    // Truy vấn pending requests
        //    var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery()
        //        .Where(r => r.Status == "UNA")
        //        .WhereIf(!string.IsNullOrEmpty(keyword), r => r.Code.ToLower().Contains(keyword) || r.Description.ToLower().Contains(keyword))
        //        .AsNoTracking();

        //    // Pending CREATE (lấy ngoài LINQ để tránh Union sinh lỗi charset)
        //    var pendingCreates = await proposedBranchQuery
        //        .Where(r => r.Action == "CREATE")
        //        .Select(r => new RolePagingDto
        //        {
        //            Id = null,
        //            Name = r.Name,
        //            Code = r.Code,
        //            IsActive = r.IsActive,
        //            Description = r.Description,
        //            Status = "PENDING",
        //            RequestType = "CREATE"
        //        })
        //        .ToListAsync();

        //    // Role có overlay (UPDATE/DELETE)
        //    var rolesWithOverlay = await roleQuery
        //        .GroupJoin(proposedBranchQuery,
        //            role => role.Id,
        //            req => req.EntityId,
        //            (role, reqs) => new { role, req = reqs.FirstOrDefault() })
        //        .Select(x => new RolePagingDto
        //        {
        //            Id = x.role.Id,
        //            Name = x.role.Name,
        //            Code = x.role.Code,
        //            IsActive = x.role.IsActive,
        //            Description = x.role.Description,
        //            Status = x.req == null ? "APPROVED" : "PENDING",
        //            RequestType = x.req == null ? null : x.req.Action
        //        })
        //        .ToListAsync();

        //    // Gộp lại thủ công, phân trang bằng LINQ in-memory
        //    var combinedList = rolesWithOverlay
        //        .Concat(pendingCreates)
        //        .OrderBy(dto => dto.Status == "PENDING" ? 0 : 1)
        //        .ThenBy(dto => dto.Id)
        //        .ToList();

        //    var total = combinedList.Count;
        //    var items = combinedList
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToList();

        //    return PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);
        //}


        //public async Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request)
        //{
        //    // Dùng biến ngoài để ép EF sử dụng parameter đúng kiểu
        //    var statusApproved = StatusConstant.Approved;
        //    var statusPending = StatusConstant.Pending;
        //    var requestCreate = RequestTypeConstant.Create;

        //    var keyword = request?.Keyword?.Trim();
        //    int pageIndex = Math.Max(1, request.PageIndex ?? 1);
        //    int pageSize = Math.Max(1, request.PageSize ?? 10);

        //    // ===== TRUY VẤN CHÍNH =====
        //    var roleQuery = _roleManager.Roles
        //        .WhereIf(!string.IsNullOrEmpty(keyword),
        //            x => EF.Functions.Like(x.Code, $"%{keyword}%") ||
        //                 EF.Functions.Like(x.Description, $"%{keyword}%"))
        //        .AsNoTracking();

        //    var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery()
        //        .Where(r => r.Status == RequestStatusConstant.Unauthorised)
        //        .WhereIf(!string.IsNullOrEmpty(keyword),
        //            r => EF.Functions.Like(r.Code, $"%{keyword}%") ||
        //                 EF.Functions.Like(r.Description, $"%{keyword}%"))
        //        .AsNoTracking();

        //    // ===== TẠO DANH SÁCH PENDING CREATE =====
        //    var pendingCreatesQuery = proposedBranchQuery
        //        .Where(r => r.Action == requestCreate)
        //        .Select(r => new
        //        {
        //            r.Name,
        //            r.Code,
        //            r.IsActive,
        //            r.Description
        //        })
        //        .Select(x => new RolePagingDto
        //        {
        //            Id = null,
        //            Name = x.Name,
        //            Code = x.Code,
        //            IsActive = x.IsActive,
        //            Description = x.Description,
        //            Status = statusPending,
        //            RequestType = requestCreate
        //        });

        //    // ===== GHÉP ROLE VỚI REQUEST (PENDING UPDATE/DELETE) =====
        //    var rolesWithOverlayQuery = roleQuery
        //        .GroupJoin(proposedBranchQuery,
        //            role => role.Id,
        //            req => req.EntityId,
        //            (role, reqs) => new { role, req = reqs.FirstOrDefault() })
        //        .Select(x => new RolePagingDto
        //        {
        //            Id = x.role.Id,
        //            Name = x.role.Name,
        //            Code = x.role.Code,
        //            IsActive = x.role.IsActive,
        //            Description = x.role.Description,
        //            Status = x.req == null ? statusApproved : statusPending,
        //            RequestType = x.req == null ? null : x.req.Action
        //        });

        //    // ===== GHÉP DANH SÁCH CUỐI & SẮP XẾP =====
        //    var combinedQuery = rolesWithOverlayQuery
        //        .Concat(pendingCreatesQuery) // dùng Concat thay Union để tránh cast sai kiểu
        //        .OrderBy(dto => dto.Status == statusPending ? 0 : 1)
        //        .ThenBy(dto => dto.Id);

        //    // ===== PHÂN TRANG =====
        //    var total = await combinedQuery.CountAsync();
        //    var items = await combinedQuery
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);
        //}


        public async Task<RoleDto?> GetRoleByIdAsync(long id)
        {
            var role = await _roleManager.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return null;

            var claims = await _roleManager.GetClaimsAsync(role);
            var pendingRequest = await _roleRequestRepository.GetBranchCombinedQuery().Where(r => r.EntityId == id && r.Status == RequestStatusConstant.Unauthorised).FirstOrDefaultAsync();

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
        public async Task<long> CreateAddRoleRequestAsync(CreateRoleDto dto)
        {
            // Serialize đề xuất
            var requestedJson = JsonSerializer.Serialize(dto);

            // Lấy người tạo từ context (hoặc truyền vào)
            var requestedBy = "system"; // cần ICurrentUser nếu có

            var request = new RoleRequest
            {
                Action = RequestTypeConstant.Create,
                Status = RequestStatusConstant.Unauthorised, // PENDING
                EntityId = 0,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                RequestedData = requestedJson
            };

            await _roleRequestRepository.CreateAsync(request);
            return request.Id;
        }
        public async Task<long> CreateUpdateRoleRequestAsync(long roleId, UpdateRoleDto dto)
        {
            // Kiểm tra role tồn tại
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
                throw new Exception("Role not found");

            // (Optional) Kiểm tra nếu đã có yêu cầu PENDING
            var existingRequest = await _roleRequestRepository.FindAll()
                .AnyAsync(r => r.EntityId == roleId &&
                               r.Status == RequestStatusConstant.Unauthorised &&
                               r.Action == RequestTypeConstant.Update);

            if (existingRequest)
                throw new Exception("A pending update request already exists for this role.");

            // Serialize dữ liệu đề xuất
            var requestedJson = JsonSerializer.Serialize(dto);
            var requestedBy = "system";

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
        public async Task<long> CreateDeleteRoleRequestAsync(long roleId)
        {
            // Kiểm tra role có tồn tại không
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
                throw new Exception("Role not found.");

            // (Optional) Kiểm tra nếu đã có yêu cầu xóa đang chờ duyệt
            var hasPendingDelete = await _roleRequestRepository.FindAll()
                .AnyAsync(r => r.EntityId == roleId &&
                               r.Status == RequestStatusConstant.Unauthorised &&
                               r.Action == RequestTypeConstant.Delete);

            if (hasPendingDelete)
                throw new Exception("A pending delete request already exists for this role.");

            // Lấy claims hiện tại của role
            var claims = await _roleManager.GetClaimsAsync(role);

            // Tạo RoleDto để lưu lại trạng thái hiện tại
            var currentSnapshot = new RoleDto
            {
                Id = role.Id,
                Code = role.Code,
                Name = role.Name,
                Description = role.Description,
                //Claims = claims.Select(c => new ClaimDto
                //{
                //    Type = c.Type,
                //    Value = c.Value
                //}).ToList()
            };

            var requestedBy = "system";

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
        public async Task CancelRoleRequestAsync(long requestId)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
                throw new Exception("Pending request not found or already processed.");

            var currentUser = "system";

            if (request.CheckerId != currentUser)
                throw new Exception("You can only cancel your own request.");

            request.Status = RequestStatusConstant.Cancelled;
            request.MakerId = currentUser;
            request.ApproveDate = DateTime.UtcNow;

            await _roleRequestRepository.UpdateAsync(request);
        }
        #endregion
        public Task AddClaimsAsync(long roleId, IEnumerable<ClaimDto> claims)
        {
            throw new NotImplementedException();
        }
        public Task<long> CreateAsync(CreateRoleDto dto)
        {
            throw new NotImplementedException();
        }
        public Task<long> CreateFromApprovedRequestAsync(string requestedDataJson)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<ClaimDto>> GetClaimsAsync(long roleId)
        {
            throw new NotImplementedException();
        }
        public Task RemoveClaimAsync(long roleId, ClaimDto claim)
        {
            throw new NotImplementedException();
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
    }
}
