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

namespace Flex.AspNetIdentity.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRoleRequestRepository _roleRequestRepository;

        public RoleService(ILogger<RoleService> logger, IRoleRequestRepository roleRequestRepository, RoleManager<Role> roleManager)
        {
            _logger = logger;
            _roleRequestRepository = roleRequestRepository;
            _roleManager = roleManager;
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

        public Task<RoleDto?> GetRoleByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public Task<string?> CompareRoleWithRequestAsync(long requestId)
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
