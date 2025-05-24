using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Flex.Shared.SeedWork.Workflow.Constants;
using Microsoft.EntityFrameworkCore;
using Flex.Infrastructure.EF;
using Flex.Shared.Constants.Common;
using Flex.Shared.DTOs.System.Branch;

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
        public async Task<PagedResult<RolePagingDto>> GetRolePagedAsync(GetRolesPagingRequest request)
        {
            var keyword = request?.Keyword?.Trim().ToLower();

            // ========== PAGING ==========
            var roleQuery = _roleManager.Roles.AsNoTracking();
            var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery();

            // Filter query
            var approvedRolesQuery = roleQuery
                .WhereIf(!string.IsNullOrEmpty(request?.Keyword), b => b.Code.ToLower().Contains(keyword) || b.Description.ToLower().Contains(keyword))
                .Select(role => new RolePagingDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    Status = "APPROVED"
                });

            var proposedRolesQuery = proposedBranchQuery
                .WhereIf(!string.IsNullOrEmpty(request?.Keyword), b => b.Code.ToLower().Contains(keyword) || b.Description.ToLower().Contains(keyword))
                .Where(r => r.Status == RequestStatusConstant.Unauthorised)
                .Select(r => new RolePagingDto
                {
                    Id = r.Id,
                    Name = r.Code,
                    Description = r.Name,
                    Status = "PENDING"
                });

            // Combined and paging
            var combinedQuery = approvedRolesQuery.Union(proposedRolesQuery);
            var total = await approvedRolesQuery.CountAsync();
            int pageIndex = request.PageIndex == null ? 1 : request.PageIndex.Value;
            int pageSize = request.PageSize == null ? total : request.PageSize.Value;

            var items = await combinedQuery
            .OrderBy(x => x.Status != StatusConstant.Approved ? 1 : 0)
            .ThenBy(x => x.Id)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var resp = PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);

            return resp;
        }

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
    }
}
