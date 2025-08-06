using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Infrastructure.EF;
using Flex.Shared.Constants.Common;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow.Constants;

namespace Flex.AspNetIdentity.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRoleRequestRepository _roleRequestRepository;
        private readonly IUserService _userService;

        public RoleService(ILogger<RoleService> logger, IRoleRequestRepository roleRequestRepository, RoleManager<Role> roleManager, IUserService userService)
        {
            _logger = logger;
            _roleRequestRepository = roleRequestRepository;
            _roleManager = roleManager;
            _userService = userService;
        }
        #region Query

        /// <summary>
        /// Get all approved roles with pagination.
        /// </summary>
        public async Task<PagedResult<RoleApprovedListItemDto>> GetApprovedRolesPagedAsync(GetRolesPagingRequest request)
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
        public async Task<RoleDto> GetApprovedRoleByCodeAsync(string code)
        {
            // ===== Find role by code =====
            var role = await _roleManager.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Code == code);

            if (role == null)
            {
                throw new Exception($"Role with code '{code}' not exists.");
            }

            // ===== If role exists, get its claims =====
            var claims = await _roleManager.GetClaimsAsync(role);

            var result = new RoleDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty,
                Code = role.Code,
                IsActive = role.IsActive,
                Description = role.Description,
                Claims = claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList(),
                Status = StatusConstant.Approved,
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
        public async Task<PagedResult<RolePagingDto>> GetPendingRolesPagedAsync(GetRolesPagingRequest request)
        {
            // ===== Process request parameters =====
            var keyword = request?.Keyword?.Trim();
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            var proposedBranchQuery = _roleRequestRepository.GetBranchCombinedQuery()
                .Where(r => r.Status == RequestStatusConstant.Unauthorised)
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    r => EF.Functions.Like(r.Code, $"%{keyword}%") ||
                         EF.Functions.Like(r.Description, $"%{keyword}%"));
            
            proposedBranchQuery = proposedBranchQuery.AsNoTracking();

            var pendingQuery = proposedBranchQuery
                .Select(r => new RolePagingDto
                {
                    Id = null,
                    Code = r.Code,
                    Name = r.Name,
                    IsActive = r.IsActive,
                    Description = r.Description,
                    Status = r.Status,
                    RequestType = r.Action,
                    RequestId = r.Id,
                    RequestedBy = r.CreatedBy,
                    RequestedDate = r.CreatedDate,
                    ApprovedBy = null,
                    ApprovedDate = null
                });

            var sortedQuery = pendingQuery.OrderByDescending(dto => dto.RequestedDate).ThenBy(dto => dto.Id);

            var total = await sortedQuery.CountAsync();
            var items = await sortedQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PagedResult<RolePagingDto>.Create(pageIndex, pageSize, total, items);
        }

        /// <summary>
        /// Lấy thông tin chi tiết request để hiển thị trong modal
        /// Trả về oldData và newData để so sánh
        /// </summary>
        public async Task<RoleRequestDetailDto?> GetRoleRequestDetailAsync(long requestId)
        {
            var request = await _roleRequestRepository.FindAll().FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return null;

            var result = new RoleRequestDetailDto
            {
                RequestId = request.Id.ToString(),
                Type = request.Action,
                CreatedBy = request.MakerId,
                CreatedDate = request.RequestedDate.ToString("yyyy-MM-dd")
            };

            // Xử lý theo loại request
            switch (request.Action)
            {
                case RequestTypeConstant.Create:
                    // Chỉ có newData
                    var createData = string.IsNullOrEmpty(request.RequestedData) ? null : JsonSerializer.Deserialize<CreateRoleRequestDto>(request.RequestedData);
                    
                    if (createData != null)
                    {
                        result.NewData = new RoleDetailDataDto
                        {
                            RoleCode = createData.Code,
                            RoleName = createData.Name,
                            Description = createData.Description,
                            Permissions = createData.Claims?.Select(c => $"{c.Type}:{c.Value}").ToList() ?? new List<string>()
                        };
                    }
                    break;

                case RequestTypeConstant.Update:
                    // Có cả oldData và newData
                    var updateData = string.IsNullOrEmpty(request.RequestedData) ? null : JsonSerializer.Deserialize<UpdateRoleRequestDto>(request.RequestedData);

                    if (updateData != null)
                    {
                        // Lấy thông tin cũ từ bảng chính
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

                        result.NewData = new RoleDetailDataDto
                        {
                            RoleCode = "",
                            RoleName = updateData.Name,
                            Description = updateData.Description,
                            Permissions = updateData.Claims?.Select(c => $"{c.Type}:{c.Value}").ToList() ?? new List<string>()
                        };
                    }
                    break;

                case RequestTypeConstant.Delete:
                    // Chỉ có oldData (thông tin sẽ bị xóa)
                    var deleteData = string.IsNullOrEmpty(request.RequestedData) ? null : JsonSerializer.Deserialize<RoleDto>(request.RequestedData);

                    if (deleteData != null)
                    {
                        result.OldData = new RoleDetailDataDto
                        {
                            RoleCode = deleteData.Code,
                            RoleName = deleteData.Name,
                            Description = deleteData.Description,
                            Permissions = deleteData.Claims?.Select(c => $"{c.Type}:{c.Value}").ToList() ?? new List<string>()
                        };
                    }
                    break;
            }

            return result;
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
                Claims = claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList()
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
            await _roleManager.UpdateAsync(role);

            return request.Id;
        }
        public async Task ApproveRoleRequestAsync(long requestId, string? comment = null, string? approvedBy = null)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
                throw new Exception("Pending role request not found.");

            var approver = approvedBy ?? "system";

            // Xử lý theo loại yêu cầu
            if (request.Action == RequestTypeConstant.Create)
            {
                var dto = JsonSerializer.Deserialize<CreateRoleRequestDto>(request.RequestedData);
                if (dto == null) throw new Exception("Invalid request data");

                var newRole = new Role(dto.Name, dto.Code)
                {
                    Description = dto.Description,
                    IsActive = dto.IsActive
                };

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
                var dto = JsonSerializer.Deserialize<UpdateRoleRequestDto>(request.RequestedData);
                if (dto == null) throw new Exception("Invalid request data");

                var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId);
                if (role == null) throw new Exception("Original role not found");

                //role.Name = dto.Name;
                //role.Description = dto.Description;
                //role.LastModifiedBy = approver;
                //role.LastModifiedDate = DateTime.UtcNow;

                // Update role status to AUT (Authorised) when update is approved
                role.Status = RequestStatusConstant.Authorised;
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
            request.Status = RequestStatusConstant.Authorised;
            request.CheckerId = approver;
            request.ApproveDate = DateTime.UtcNow;
            request.Comments = comment;

            await _roleRequestRepository.UpdateAsync(request);
        }
        public async Task RejectRoleRequestAsync(long requestId, string reason, string rejectedBy)
        {
            var request = await _roleRequestRepository
                .FindAll()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatusConstant.Unauthorised);

            if (request == null)
                throw new Exception("Pending request not found.");

            // If this is an update or delete request, revert the role status back to APPROVED
            if ((request.Action == RequestTypeConstant.Update || request.Action == RequestTypeConstant.Delete) && request.EntityId > 0)
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId);
                if (role != null)
                {
                    role.Status = StatusConstant.Approved; // Revert to approved status
                    await _roleManager.UpdateAsync(role);
                }
            }

            // Cập nhật trạng thái và thông tin từ chối
            request.Status = RequestStatusConstant.Rejected;
            request.CheckerId = rejectedBy;
            request.ApproveDate = DateTime.UtcNow;
            request.Comments = reason;

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

            // If this is an update or delete request, revert the role status back to APPROVED
            if ((request.Action == RequestTypeConstant.Update || request.Action == RequestTypeConstant.Delete) && request.EntityId > 0)
            {
                var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == request.EntityId);
                if (role != null)
                {
                    role.Status = StatusConstant.Approved; // Revert to approved status
                    await _roleManager.UpdateAsync(role);
                }
            }

            request.Status = RequestStatusConstant.Cancelled;
            request.CheckerId = currentUser;
            request.ApproveDate = DateTime.UtcNow;
            request.Comments = "Cancelled by maker.";

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
        public Task<long> CreateAsync(CreateRoleRequestDto dto)
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
        public Task UpdateAsync(long roleId, UpdateRoleRequestDto dto)
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
                    RequestId = r.Id ?? 0,
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
