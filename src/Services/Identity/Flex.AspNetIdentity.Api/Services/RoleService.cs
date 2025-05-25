using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Flex.Infrastructure.EF;
using Flex.Shared.Constants.Common;
using System.Text.Json;
using System.Security.Claims;

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
        public async Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request)
        {
            var keyword = request?.Keyword?.Trim().ToLower();
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ========== QUERY ==========
            var roleQuery = _roleManager.Roles.AsNoTracking();
            var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery().Where(r => r.Status == RequestStatusConstant.Unauthorised).Where(r => r.Status == RequestStatusConstant.Unauthorised);

            // ========== PENDING CREATE ==========
            var pendingCreates = proposedBranchQuery
                .Where(r => r.RequestType == RequestTypeConstant.Create)
                .WhereIf(!string.IsNullOrEmpty(keyword), r => r.Code.ToLower().Contains(keyword) || r.Description.ToLower().Contains(keyword))
                .Select(r => new RolePagingDto
                {
                    Id = null,
                    Name = r.Name,
                    Code = r.Code,
                    Description = r.Description,
                    Status = StatusConstant.Pending,
                    RequestType = RequestTypeConstant.Create
                });

            // ========== ROLES ==========
            var rolesWithOverlay = roleQuery
                .GroupJoin(
                    proposedBranchQuery.Where(r => r.RequestType != RequestTypeConstant.Create),
                    role => role.Id,
                    req => req.EntityId,
                    (role, reqs) => new { role, req = reqs.FirstOrDefault() })
                .WhereIf(!string.IsNullOrEmpty(keyword), x => x.role.Code.ToLower().Contains(keyword) || x.role.Description.ToLower().Contains(keyword))
                .Select(x => new RolePagingDto
                {
                    Id = x.role.Id,
                    Name = x.role.Name,
                    Code = x.role.Code,
                    Description = x.role.Description,
                    Status = x.req == null ? StatusConstant.Approved : StatusConstant.Pending,
                    RequestType = x.req == null ? null : x.req.RequestType
                });

            // ========== UNION && ORDER ==========
            var combined = rolesWithOverlay.Union(pendingCreates)
                .OrderBy(dto => dto.Status == StatusConstant.Pending ? 0 : 1)
                .ThenBy(dto => dto.Id);

            // ========== PAGINATION ==========
            var total = await combined.CountAsync();
            var items = await combined
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();

            var resp = PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);

            return resp;
        }
        public async Task<RoleDto?> GetRoleByIdAsync(long id)
        {
            var role = await _roleManager.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return null;

            var claims = await _roleManager.GetClaimsAsync(role);

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Code = role.Code,
                Description = role.Description,
                Claims = claims.Select(c => new ClaimDto
                {
                    Type = c.Type,
                    Value = c.Value
                }).ToList()
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
        public async Task<List<RoleImpactDto>> GetRoleRequestImpactAsync(long requestId)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
                throw new Exception("Role request not found");

            if (request.Action == RequestTypeConstant.Create)
                return new List<RoleImpactDto>(); // Tạo mới không ảnh hưởng gì

            var roleId = request.EntityId;

            if (request.Action == RequestTypeConstant.Delete)
            {
                // Lấy danh sách user đang dùng role
                var impactedUsers = await _userManager.Users
                    //.Where(u => u.Roles.Any(ur => ur.RoleId == roleId)) // sửa theo hệ thống của bạn
                    .Select(u => new RoleImpactDto
                    {
                        ImpactType = "User",
                        Name = u.FullName,
                        Code = u.UserName,
                        Description = $"User '{u.UserName}' is using this role"
                    })
                    .ToListAsync();

                return impactedUsers;
            }

            if (request.Action == RequestTypeConstant.Update)
            {
                // Deserialize proposed role
                var proposed = JsonSerializer.Deserialize<RoleDto>(request.RequestedData ?? string.Empty);
                if (proposed == null)
                    return new List<RoleImpactDto>();

                // Lấy role hiện tại và claims hiện tại
                var role = await _roleManager.Roles
                    .FirstOrDefaultAsync(r => r.Id == roleId);

                if (role == null)
                    return new List<RoleImpactDto>();

                var currentClaims = await _roleManager.GetClaimsAsync(role);
                var proposedClaims = proposed.Claims ?? new List<ClaimDto>();

                // So sánh các claims bị xóa
                var removedClaims = currentClaims
                    .Where(c => !proposedClaims.Any(p => p.Type == c.Type && p.Value == c.Value))
                    .Select(c => new RoleImpactDto
                    {
                        ImpactType = "Claim",
                        Name = c.Type,
                        Code = c.Value,
                        Description = $"Claim {c.Type}:{c.Value} will be removed"
                    });

                return removedClaims.ToList();
            }

            return new List<RoleImpactDto>();
        }
        public async Task<string?> CompareRoleWithRequestAsync(long requestId)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || string.IsNullOrEmpty(request.RequestedData))
                return null;

            var proposed = JsonSerializer.Deserialize<RoleDto>(request.RequestedData);
            if (proposed == null)
                return null;

            var role = request.EntityId == null
                ? await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId)
                : null;

            var diffs = new List<FieldDiffDto>();

            // ===== So sánh các trường cơ bản =====
            if (role != null)
            {
                if (role.Name != proposed.Name)
                    diffs.Add(new FieldDiffDto { Field = "Name", Original = role.Name, Proposed = proposed.Name });

                if (role.Description != proposed.Description)
                    diffs.Add(new FieldDiffDto { Field = "Description", Original = role.Description, Proposed = proposed.Description });

                if (role.Code != proposed.Code)
                    diffs.Add(new FieldDiffDto { Field = "Code", Original = role.Code, Proposed = proposed.Code });
            }
            else
            {
                // Tạo mới
                diffs.Add(new FieldDiffDto { Field = "Name", Original = null, Proposed = proposed.Name });
                diffs.Add(new FieldDiffDto { Field = "Description", Original = null, Proposed = proposed.Description });
                diffs.Add(new FieldDiffDto { Field = "Code", Original = null, Proposed = proposed.Code });
            }

            // ===== So sánh claims =====
            var currentClaims = role != null ? await _roleManager.GetClaimsAsync(role) : new List<Claim>();
            var proposedClaims = proposed.Claims ?? new List<ClaimDto>();

            var removedClaims = currentClaims
                .Where(c => !proposedClaims.Any(p => p.Type == c.Type && p.Value == c.Value))
                .Select(c => new FieldDiffDto
                {
                    Field = "Claim",
                    Original = $"{c.Type}:{c.Value}",
                    Proposed = null
                });

            var addedClaims = proposedClaims
                .Where(p => !currentClaims.Any(c => c.Type == p.Type && c.Value == p.Value))
                .Select(p => new FieldDiffDto
                {
                    Field = "Claim",
                    Original = null,
                    Proposed = $"{p.Type}:{p.Value}"
                });

            diffs.AddRange(removedClaims);
            diffs.AddRange(addedClaims);

            return JsonSerializer.Serialize(diffs, new JsonSerializerOptions
            {
                WriteIndented = true
            });
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

        public Task DeleteAsync(long roleId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto?> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<RoleDto?> GetBySystemNameAsync(string systemName)
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

        public Task<long> CreateAddRoleRequestAsync(CreateRoleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<long> CreateUpdateRoleRequestAsync(long roleId, UpdateRoleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<long> CreateDeleteRoleRequestAsync(long roleId)
        {
            throw new NotImplementedException();
        }

        public Task ApproveRoleRequestAsync(long requestId, string? comment = null)
        {
            throw new NotImplementedException();
        }

        public Task RejectRoleRequestAsync(long requestId, string reason)
        {
            throw new NotImplementedException();
        }

        public Task CancelRoleRequestAsync(long requestId)
        {
            throw new NotImplementedException();
        }
    }
}
