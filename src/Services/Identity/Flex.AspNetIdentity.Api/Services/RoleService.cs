using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Models.Permission;
using Flex.AspNetIdentity.Api.Models.Role;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Infrastructure.EF;
using Flex.Shared.Authorization;
using Flex.Shared.Cache;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using System.Text.Json;

namespace Flex.AspNetIdentity.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRoleRequestRepository _roleRequestRepository;
        private readonly IUserService _userService;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IDistributedCache _cache;
        private const string ClaimTypePermission = "PERMISSION";

        public RoleService(
            ILogger<RoleService> logger,
            IRoleRequestRepository roleRequestRepository,
            RoleManager<Role> roleManager,
            IUserService userService,
            IPermissionRepository permissionRepository,
            IDistributedCache cache)
        {
            _logger = logger;
            _roleRequestRepository = roleRequestRepository;
            _roleManager = roleManager;
            _userService = userService;
            _permissionRepository = permissionRepository;
            _cache = cache;
        }
        #region Query

        /// <summary>
        /// Get all approved roles with pagination.
        /// </summary>
        public async Task<PagedResult<RoleApprovedListItemDto>> GetApprovedRolesPagedAsync(GetApproveRolesPagingRequest request)
        {
            // ===== Process request parameters =====
            var keyword = request?.Keyword?.Trim().ToLower();
            var status = request?.IsActive?.Trim().ToUpper() == "Y" ? true : false;
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== Build query =====
            var roleQuery = _roleManager.Roles
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    x => EF.Functions.Like(x.Code.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(x.Description.ToLower(), $"%{keyword}%"))
                .WhereIf(!string.IsNullOrEmpty(request.IsActive),x => x.IsActive == status);

            // ===== Execute query =====
            var total = await roleQuery.CountAsync();
            var items = await roleQuery
                .OrderBy(x => x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new RoleApprovedListItemDto
                {
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive
                })
                .ToListAsync();

            // ===== Return result =====
            return PagedResult<RoleApprovedListItemDto>.Create(pageIndex, pageSize, total, items);
        }

        /// <summary>
        /// Get approved role by code.
        /// </summary>
        public async Task<RoleDto> GetApprovedRoleByCodeAsync(string code, bool includeClaims = true, bool includeTree = true, string? search = null, CancellationToken ct = default)
        {
            // ===== Find role by code =====
            var role = await _roleManager.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Code == code);

            if (role == null)
            {
                throw new Exception($"Role with code '{code}' not exists.");
            }

            // ===== If role exists, get its claims =====
            var flags = await GetPermissionFlagsAsync(code, ct);

            var result = new RoleDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Code = role.Code,
                IsActive = role.IsActive,
                Description = role.Description,
                Status = RequestStatusConstant.Authorised,
                PermissionTree = includeTree ? flags.Root : new List<PermissionNodeDto>()
            };

            return result;
        }

        /// <summary>
        /// Get approved role history by code.
        /// </summary>
        public async Task<List<RoleChangeHistoryDto>> GetApprovedRoleChangeHistoryAsync(string roleCode)
        {
            // ===== Find role with code =====
            var role = await _roleManager.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Code == roleCode);

            if (role == null)
            {
                throw new Exception($"Role with code '{roleCode}' not exists.");
            }

            // ===== Get role histories by role Id =====
            var roleId = role.Id;

            var requests = await _roleRequestRepository.FindAll()
                .Where(r => r.EntityId == roleId).AsNoTracking()
                .OrderByDescending(r => r.RequestedDate)
                .Select(r => new
                {
                    r.Id,
                    r.MakerId,
                    r.RequestedDate,
                    r.CheckerId,
                    r.ApproveDate,
                    r.Status,
                    r.Comments,
                    r.RequestedData
                })
                .AsNoTracking()
                .ToListAsync();

            var historyItems = requests.Select((req, idx) => new RoleChangeHistoryDto
            {
                Id = req.Id,
                MakerBy = req.MakerId,
                MakerTime = req.RequestedDate,
                ApproverBy = req.CheckerId,
                ApproverTime = req.ApproveDate,
                Status = req.Status,
                Description = req.Comments,
                Changes = req.RequestedData
            }).ToList();

            return historyItems;
        }

        /// <summary>
        /// Get all pending roles with pagination.
        /// </summary>
        public async Task<PagedResult<RolePendingPagingDto>> GetPendingRolesPagedAsync(GetApproveRolesPagingRequest request)
        {
            // ===== Process request parameters =====
            var keyword = request?.Keyword?.Trim().ToLower();
            var requestType = request?.Type?.Trim().ToUpper();
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            // ===== Build query =====
            var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery()
                .Where(r => r.Status == RequestStatusConstant.Unauthorised)
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    r => EF.Functions.Like(r.Code.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(r.Description.ToLower(), $"%{keyword}%"))
                .WhereIf(!string.IsNullOrEmpty(requestType) && requestType != RequestTypeConstant.All,
                    r => r.Action == requestType)
                .AsNoTracking();

            var pendingQuery = proposedBranchQuery
                .Select(r => new RolePendingPagingDto
                {
                    Code = r.Code,
                    Name = r.Name,
                    Description = r.Description,
                    RequestType = r.Action,
                    RequestedBy = r.CreatedBy,
                    RequestedDate = r.CreatedDate,
                    RequestId = r.Id
                });

            // ===== Execute query =====
            var total = await pendingQuery.CountAsync();
            var items = await pendingQuery
                .OrderByDescending(dto => dto.RequestedDate)
                .ThenBy(dto => dto.RequestId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ===== Return result =====
            return PagedResult<RolePendingPagingDto>.Create(pageIndex, pageSize, total, items);
        }

        /// <summary>
        /// Get pending role request detail by request ID.
        /// </summary>
        public async Task<RoleRequestDetailDto> GetPendingRoleByIdAsync(long requestId)
        {
            // ===== Validation =====
            // ===== Get request data =====
            var request = await _roleRequestRepository.FindAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                throw new Exception($"Role request with ID '{requestId}' not found.");
            }

            // ===== Build base result =====
            var result = new RoleRequestDetailDto
            {
                RequestId = request.Id.ToString(),
                Type = request.Action,
                CreatedBy = request.MakerId,
                CreatedDate = request.RequestedDate.ToString("yyyy-MM-dd")
            };

            // ===== Process by request type =====
            switch (request.Action)
            {
                case RequestTypeConstant.Create:
                    ProcessCreateRequestData(request, result);
                    break;

                case RequestTypeConstant.Update:
                    await ProcessUpdateRequestData(request, result);
                    break;

                case RequestTypeConstant.Delete:
                    ProcessDeleteRequestData(request, result);
                    break;

                default:
                    throw new Exception($"Unsupported request type: {request.Action}");
            }

            // ===== Return result =====
            return result;
        }

        private static void ProcessCreateRequestData(RoleRequest request, RoleRequestDetailDto result)
        {
            if (string.IsNullOrEmpty(request.RequestedData)) return;

            var createData = JsonSerializer.Deserialize<CreateRoleRequestDto>(request.RequestedData);
            if (createData == null) return;

            result.NewData = new RoleDetailDataDto
            {
                RoleCode = createData.Code,
                RoleName = createData.Name,
                Description = createData.Description,
                //Permissions = createData.Claims?.Select(c => $"{c.Type}:{c.Value}").ToList() ?? new List<string>()
            };
        }

        private async Task ProcessUpdateRequestData(RoleRequest request, RoleRequestDetailDto result)
        {
            if (string.IsNullOrEmpty(request.RequestedData)) return;

            var updateData = JsonSerializer.Deserialize<UpdateRoleRequestDto>(request.RequestedData);
            if (updateData == null) return;

            // ===== Get current role data =====
            var currentRole = await _roleManager.Roles
                .Where(r => r.Id == request.EntityId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (currentRole != null)
            {
                var currentClaims = await _roleManager.GetClaimsAsync(currentRole);
                result.OldData = new RoleDetailDataDto
                {
                    RoleCode = currentRole.Code,
                    RoleName = currentRole.Name,
                    Description = currentRole.Description,
                    Permissions = currentClaims.Select(c => $"{c.Type}:{c.Value}").ToList()
                };
            }

            // ===== Set proposed data =====
            result.NewData = new RoleDetailDataDto
            {
                RoleCode = updateData.Code ?? string.Empty,
                RoleName = updateData.Name,
                Description = updateData.Description,
                //Permissions = updateData.Claims?.Select(c => $"{c.Type}:{c.Value}").ToList() ?? new List<string>()
            };
        }

        private static void ProcessDeleteRequestData(RoleRequest request, RoleRequestDetailDto result)
        {
            if (string.IsNullOrEmpty(request.RequestedData)) return;

            var deleteData = JsonSerializer.Deserialize<RoleDto>(request.RequestedData);
            if (deleteData == null) return;

            result.OldData = new RoleDetailDataDto
            {
                RoleCode = deleteData.Code,
                RoleName = deleteData.Name,
                Description = deleteData.Description,
            };
        }

        /// <summary>
        /// Get tree of permission flags for a specific role.
        /// </summary>
        public async Task<PermissionFlagsResult> GetPermissionFlagsAsync(string? roleCode, CancellationToken ct = default)
        {
            // ===== Get all permission ft cache =====
            var allPermissions = new List<Permission>();
            string? permissionsTreeJson = await _cache.GetStringAsync(CacheKeys.PermissionsTree);

            if (string.IsNullOrEmpty(permissionsTreeJson))
            {
                allPermissions = await _permissionRepository.GetAllAsync(ct);

                var json = JsonSerializer.Serialize(allPermissions);
                await _cache.SetStringAsync(CacheKeys.PermissionsTree, json, ct);
            }
            else
            {
                allPermissions = JsonSerializer.Deserialize<List<Permission>>(permissionsTreeJson) ?? new List<Permission>();
            }

            // ===== Filter role & permissions =====
            var all = allPermissions.Where(p => p.IsActive == PermissionStatus.Active).ToList();
            HashSet<string> selected = new HashSet<string>();

            if (string.IsNullOrWhiteSpace(roleCode))
            {
                selected = all
                    .Where(x => x.IsAssignable == 1)
                    .Select(x => x.Code)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                var role = await _roleManager.Roles.AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Code == roleCode, ct)
                    ?? throw new Exception($"Role '{roleCode}' not found.");

                var claimCodes = await _permissionRepository.GetPermissionCodesOfRoleAsync(role.Id, ct);
                selected = claimCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            // ===== Build tree permissons =====
            var byParent = all
                .GroupBy(p => p.ParentPermissionId ?? 0L)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.SortOrder).ThenBy(x => x.Id).ToList()
                );

            var root = Build(0L);

            // ===== Get info tree =====
            int total = all.Count;
            int assignable = all.Count(x => x.IsAssignable == 1);
            int checkedCount = all.Count(x => x.IsAssignable == 1 && selected.Contains(x.Code));

            return new PermissionFlagsResult
            {
                Root = root,
                Total = total,
                Assignable = assignable,
                Checked = checkedCount
            };

            // ===== Local Funcs =====
            List<PermissionNodeDto> Build(long parentId)
            {
                if (!byParent.TryGetValue(parentId, out var list)) return new();
                var nodes = new List<PermissionNodeDto>(list.Count);
                foreach (var p in list)
                {
                    var children = Build(p.Id);
                    nodes.Add(new PermissionNodeDto
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        IsAssignable = p.IsAssignable == 1,
                        IsChecked = selected.Contains(p.Code),
                        SortOrder = p.SortOrder,
                        IsCrudRule = p.IsCrudRule,
                        Children = children
                    });
                }
                return nodes;
            }
        }

        #endregion

        #region Command
        /// <summary>
        /// Create role request.
        /// </summary>
        public async Task<long> CreateRoleRequestAsync(CreateRoleRequestDto dto)
        {
            // ===== Validation =====
            // ===== Check role code is exits =====
            var existingRole = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Code == dto.Code);
            if (existingRole != null)
            {
                throw new Exception($"Role with code '{dto.Code}' already exists.");
            }

            // ===== Check role request unauthorised is exits =====
            var existingPendingRequest = await _roleRequestRepository.GetBranchCombinedQuery()
                .Where(r => r.Code == dto.Code && r.Status == RequestStatusConstant.Unauthorised)
                .FirstOrDefaultAsync();
            if (existingPendingRequest != null)
            {
                throw new Exception($"A pending request with code '{dto.Code}' already exists.");
            }

            // ===== Process =====
            var requestedBy = _userService.GetCurrentUsername() ?? "anonymous";
            var requestedJson = JsonSerializer.Serialize(dto);
            var request = new RoleRequest
            {
                Action = RequestTypeConstant.Create,
                Status = RequestStatusConstant.Unauthorised,
                EntityId = 0,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                Comments = "Yêu cầu thêm mới vai trò.",
                RequestedData = requestedJson
            };
            await _roleRequestRepository.CreateAsync(request);

            return request.Id;
        }

        /// <summary>
        /// Create update role request.
        /// </summary>
        public async Task<long> CreateUpdateRoleRequestAsync(string code, UpdateRoleRequestDto dto)
        {
            // ===== Validation =====
            // ===== Check role code is exits =====
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Code == code);
            if (role == null)
            {
                throw new Exception($"Role with code '{code}' does not exist.");
            }

            // ===== Check role request is exits =====
            if (role.Status == RequestStatusConstant.Unauthorised)
            {
                throw new Exception("A pending update request already exists for this role.");
            }

            // ===== Process =====
            // ===== Create update role request =====
            var requestedBy = _userService.GetCurrentUsername() ?? "anonymous";
            dto.Code = code;
            var requestedJson = JsonSerializer.Serialize(dto);
            var request = new RoleRequest
            {
                Action = RequestTypeConstant.Update,
                Status = RequestStatusConstant.Unauthorised,
                EntityId = role.Id,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                RequestedData = requestedJson,
                Comments = dto.Comment ?? "Yêu cầu cập nhật vai trò."
            };
            // ===== Update status process role =====
            role.Status = RequestStatusConstant.Unauthorised;

            // ===== Transaction =====
            await using var transaction = await _roleRequestRepository.BeginTransactionAsync();
            try
            {
                await _roleRequestRepository.CreateAsync(request);
                await _roleManager.UpdateAsync(role);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to create update role request.");
            }

            return request.Id;
        }

        /// <summary>
        /// Create delete role request.
        /// </summary>
        public async Task<long> CreateDeleteRoleRequestAsync(string code, DeleteRoleRequestDto dto)
        {
            // ===== Validation =====
            // ===== Check role code is exits =====
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Code == code);
            if (role == null)
            {
                throw new Exception($"Role with code '{code}' does not exist.");
            }

            // ===== Check role request is exits =====
            if (role.Status == RequestStatusConstant.Unauthorised)
            {
                throw new Exception("A pending update request already exists for this role.");
            }

            // ===== Process =====
            // ===== Create delete role request =====
            var claims = await _roleManager.GetClaimsAsync(role);
            var currentSnapshot = new RoleDto
            {
                Id = role.Id,
                Code = role.Code,
                Name = role.Name,
                Description = role.Description,
            };
            var requestedBy = _userService.GetCurrentUsername() ?? "anonymous";
            var request = new RoleRequest
            {
                Action = RequestTypeConstant.Delete,
                Status = RequestStatusConstant.Unauthorised,
                EntityId = role.Id,
                MakerId = requestedBy,
                RequestedDate = DateTime.UtcNow,
                RequestedData = JsonSerializer.Serialize(currentSnapshot),
                Comments = dto.Comment ?? "Yêu cầu xóa vai trò."
            };

            // ===== Update status process role =====
            role.Status = RequestStatusConstant.Unauthorised;

            // ===== Transaction =====
            await using var transaction = await _roleRequestRepository.BeginTransactionAsync();
            try
            {
                await _roleRequestRepository.CreateAsync(request);
                await _roleManager.UpdateAsync(role);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to create update role request.");
            }

            return request.Id;
        }

        /// <summary>
        /// Approve pending role request with comprehensive validation and transaction handling.
        /// </summary>
        public async Task<RoleApprovalResultDto> ApprovePendingRoleRequestAsync(long requestId, string? comment = null)
        {
            // ===== Validation =====

            // ===== Get request data =====
            var request = await _roleRequestRepository
                .FindAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
            {
                throw new Exception($"Pending role request with ID '{requestId}' not found.");
            }

            // ===== Process approval with transaction =====
            var approver = _userService.GetCurrentUsername() ?? "anonymous";
            await using var transaction = await _roleRequestRepository.BeginTransactionAsync();
            try
            {
                long? createdRoleId = null;

                // ===== Process by request type =====
                switch (request.Action)
                {
                    case RequestTypeConstant.Create:
                        createdRoleId = await ProcessCreateApproval(request, approver);
                        break;

                    case RequestTypeConstant.Update:
                        await ProcessUpdateApproval(request, approver);
                        break;

                    case RequestTypeConstant.Delete:
                        await ProcessDeleteApproval(request, approver);
                        break;

                    default:
                        throw new Exception($"Unsupported request type: {request.Action}");
                }

                // ===== Update request status =====
                switch (request.Action)
                {
                    case RequestTypeConstant.Create:
                        comment = "Yêu cầu thêm mới vai trò.";
                        break;

                    case RequestTypeConstant.Update:
                        comment = "Yêu cầu cập nhật vai trò.";
                        break;

                    default:
                        throw new Exception($"Unsupported request type: {request.Action}");
                }

                await UpdateRequestStatus(request, approver, comment);

                await transaction.CommitAsync();

                // ===== Return result =====
                return new RoleApprovalResultDto
                {
                    RequestId = request.Id,
                    RequestType = request.Action,
                    Status = RequestStatusConstant.Authorised,
                    ApprovedBy = approver,
                    ApprovedDate = DateTime.UtcNow,
                    Comment = comment,
                    CreatedRoleId = createdRoleId
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to approve role request ID '{requestId}'.");
            }
        }

        private async Task<long> ProcessCreateApproval(RoleRequest request, string approver)
        {
            if (string.IsNullOrEmpty(request.RequestedData))
            {
                throw new Exception("Request data is empty for CREATE request.");
            }

            var dto = JsonSerializer.Deserialize<CreateRoleRequestDto>(request.RequestedData);
            if (dto == null)
            {
                throw new Exception("Invalid CREATE request data format.");
            }

            // ===== Create new role =====
            var newRole = new Role(dto.Name, dto.Code)
            {
                Description = dto.Description,
                IsActive = dto.IsActive,
                Status = RequestStatusConstant.Authorised
            };

            await _roleManager.CreateAsync(newRole);

            // ===== Add claims if any =====
            if (dto.Claims != null && dto.Claims.Any())
            {
                foreach (var code in dto.Claims)
                {
                    await _roleManager.AddClaimAsync(newRole, new Claim(Shared.Authorization.ClaimTypes.Permission, code));
                }
            }

            // ===== Update request with created role ID =====
            request.EntityId = newRole.Id;

            return newRole.Id;
        }

        private async Task ProcessUpdateApproval(RoleRequest request, string approver)
        {
            if (string.IsNullOrEmpty(request.RequestedData))
            {
                throw new Exception("Request data is empty for UPDATE request.");
            }

            var dto = JsonSerializer.Deserialize<UpdateRoleRequestDto>(request.RequestedData);
            if (dto == null)
            {
                throw new Exception("Invalid UPDATE request data format.");
            }

            // ===== Get existing role =====
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId);
            if (role == null)
            {
                throw new Exception($"Original role with ID '{request.EntityId}' not found.");
            }

            // ===== Update role properties =====
            role.Name = dto.Name;
            role.Description = dto.Description;
            role.IsActive = dto.IsActive;
            role.Status = RequestStatusConstant.Authorised;

            await _roleManager.UpdateAsync(role);

            // ===== Update claims =====
            if (dto.Claims != null)
            {
                var existingClaims = await _roleManager.GetClaimsAsync(role);
                // Remove existing claims
                foreach (var existingClaim in existingClaims)
                {
                    await _roleManager.RemoveClaimAsync(role, existingClaim);
                }
                // Add new claims
                foreach (var claim in dto.Claims)
                {
                    //await _roleManager.AddClaimAsync(role, new Claim(Shared.Authorization.ClaimTypes.Permission, claim.Value));
                }
            }
        }

        private async Task ProcessDeleteApproval(RoleRequest request, string approver)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId);
            if (role == null)
            {
                throw new Exception($"Role with ID '{request.EntityId}' not found for deletion.");
            }

            await _roleManager.DeleteAsync(role);
        }

        private async Task UpdateRequestStatus(RoleRequest request, string approver, string? comment)
        {
            request.Status = RequestStatusConstant.Authorised;
            request.CheckerId = approver;
            request.ApproveDate = DateTime.UtcNow;
            request.Comments = comment ?? "Approved";

            await _roleRequestRepository.UpdateAsync(request);
        }

        /// <summary>
        /// Reject pending role request with comprehensive validation and transaction handling.
        /// </summary>
        public async Task<RoleApprovalResultDto> RejectPendingRoleRequestAsync(long requestId, string? reason = null)
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
            var request = await _roleRequestRepository
                .FindAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
            {
                throw new Exception($"Pending role request with ID '{requestId}' not found.");
            }

            // ===== Process rejection with transaction =====
            await using var transaction = await _roleRequestRepository.BeginTransactionAsync();
            try
            {
                // ===== Revert role status if needed =====
                await RevertRoleStatusIfNeeded(request);

                // ===== Update request status =====
                await UpdateRejectedRequestStatus(request, rejecter, reason);

                await transaction.CommitAsync();

                // ===== Return result =====
                return new RoleApprovalResultDto
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
                throw new Exception($"Failed to reject role request ID '{requestId}'.");
            }
        }

        /// <summary>
        /// Revert role status back to approved for UPDATE/DELETE requests.
        /// </summary>
        private async Task RevertRoleStatusIfNeeded(RoleRequest request)
        {
            if ((request.Action == RequestTypeConstant.Update || request.Action == RequestTypeConstant.Delete) 
                && request.EntityId > 0)
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId);
                if (role != null)
                {
                    role.Status = RequestStatusConstant.Authorised; // Revert to approved status
                    await _roleManager.UpdateAsync(role);
                }
            }
        }

        /// <summary>
        /// Update request status after rejection.
        /// </summary>
        private async Task UpdateRejectedRequestStatus(RoleRequest request, string rejecter, string? reason)
        {
            request.Status = RequestStatusConstant.Rejected;
            request.CheckerId = rejecter;
            request.ApproveDate = DateTime.UtcNow;
            request.Comments = reason ?? "Rejected";

            await _roleRequestRepository.UpdateAsync(request);
        }
        #endregion

        /// <summary>
        /// Thay thế toàn bộ permission-claims cho role bằng set mới.
        /// Tự thêm *.VIEW nếu người dùng chỉ chọn CREATE/UPDATE/DELETE/APPROVE.
        /// </summary>
        public async Task UpdateRolePermissionsAsync(string roleCode, IEnumerable<string> permissionCodes, CancellationToken ct = default)
        {
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Code == roleCode, ct)
                       ?? throw new Exception($"Role '{roleCode}' not found.");

            // 1) Chuẩn hoá input
            var input = permissionCodes?
                .Select(c => c?.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToHashSet(StringComparer.OrdinalIgnoreCase)
                ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 2) Tự thêm *.VIEW cho các hành động
            var toAdd = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var code in input.ToList())
            {
                if (EndsWithAny(code, ".CREATE", ".UPDATE", ".DELETE", ".APPROVE"))
                {
                    var prefix = code[..code.LastIndexOf('.')];
                    var viewCode = $"{prefix}.VIEW";
                    if (!input.Contains(viewCode)) toAdd.Add(viewCode);
                }
            }
            input.UnionWith(toAdd);

            // 3) Replace-set claims (type = PERMISSION)
            var existing = await _roleManager.GetClaimsAsync(role);
            var existingPerms = existing.Where(c => c.Type.Equals(ClaimTypePermission, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var c in existingPerms)
                await _roleManager.RemoveClaimAsync(role, c);

            //foreach (var code in input.Distinct(StringComparer.OrdinalIgnoreCase))
            //    await _roleManager.AddClaimAsync(role, new Claims.Claim(ClaimTypePermission, code));
        }

        private static bool EndsWithAny(string value, params string[] suffixes)
        {
            foreach (var s in suffixes)
                if (value.EndsWith(s, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }

        public sealed class SavePermissionsRequest
        {
            public List<string> PermissionCodes { get; set; } = new();
        }
    }
}
