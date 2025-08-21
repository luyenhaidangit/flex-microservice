using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Infrastructure.EF;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flex.AspNetIdentity.Api.Services
{
    public class UserAdminService : IUserAdminService
    {
        private readonly ILogger<UserAdminService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserService _userService;

        public UserAdminService(
            ILogger<UserAdminService> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IUserService userService)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        #region Query - Approved
        public async Task<PagedResult<UserApprovedListItemDto>> GetApprovedUsersPagedAsync(GetUsersPagingRequest request)
        {
            var keyword = request?.Keyword?.Trim().ToLower();
            int pageIndex = Math.Max(1, request.PageIndex ?? 1);
            int pageSize = Math.Max(1, request.PageSize ?? 10);

            var query = _userManager.Users
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    u => EF.Functions.Like((u.UserName ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((u.Email ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((u.FullName ?? string.Empty).ToLower(), $"%{keyword}%"))
                .WhereIf(request?.BranchId != null, u => u.BranchId == request!.BranchId);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(u => u.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserApprovedListItemDto
                {
                    UserName = u.UserName ?? string.Empty,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    BranchId = u.BranchId,
                    IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow,
                    IsActive = true
                })
                .ToListAsync();

            return PagedResult<UserApprovedListItemDto>.Create(pageIndex, pageSize, total, items);
        }

        public async Task<UserDetailDto> GetApprovedUserByUserNameAsync(string userName)
        {
            var user = await _userManager.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserName == userName)
                       ?? throw new Exception($"User '{userName}' not found.");

            var roles = await _userManager.GetRolesAsync(user);
            return new UserDetailDto
            {
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BranchId = user.BranchId,
                IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow,
                IsActive = true,
                Roles = roles.ToList()
            };
        }

        public Task<List<UserChangeHistoryDto>> GetApprovedUserChangeHistoryAsync(string userName)
        {
            // Chưa có bảng history chuẩn cho User; trả về rỗng hoặc tích hợp sau với AuditLog
            return Task.FromResult(new List<UserChangeHistoryDto>());
        }
        #endregion

        #region Commands on approved
        public async Task AssignRolesAsync(string userName, IEnumerable<string> roleCodes, CancellationToken ct = default)
        {
            var user = await _userManager.FindByNameAsync(userName) 
                       ?? throw new Exception($"User '{userName}' not found.");

            // map code -> role name
            var codeSet = roleCodes?.Select(c => c?.Trim()).Where(c => !string.IsNullOrWhiteSpace(c)).ToHashSet(StringComparer.OrdinalIgnoreCase) 
                          ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var roles = await _roleManager.Roles.Where(r => codeSet.Contains(r.Code)).Select(r => r.Name!).ToListAsync(ct);

            var existing = await _userManager.GetRolesAsync(user);
            var toRemove = existing.Where(r => !roles.Contains(r)).ToList();
            var toAdd = roles.Where(r => !existing.Contains(r)).ToList();

            if (toRemove.Count > 0) await _userManager.RemoveFromRolesAsync(user, toRemove);
            if (toAdd.Count > 0) await _userManager.AddToRolesAsync(user, toAdd);
        }

        public async Task LockAsync(string userName, string? reason = null)
        {
            var user = await _userManager.FindByNameAsync(userName)
                       ?? throw new Exception($"User '{userName}' not found.");
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        }

        public async Task UnlockAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName)
                       ?? throw new Exception($"User '{userName}' not found.");
            await _userManager.SetLockoutEndDateAsync(user, null);
        }

        public async Task<string> ResetPasswordAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName)
                       ?? throw new Exception($"User '{userName}' not found.");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // TODO: gửi email chứa token theo flow ngoài scope hiện tại
            return token;
        }
        #endregion

        #region Create/Update/Delete immediate with audit comment
        public async Task<string> CreateUserAsync(CreateUserRequestDto dto, string? comment = null)
        {
            // Validate role codes
            var existingCodes = await _roleManager.Roles
                .Where(r => dto.RoleCodes.Contains(r.Code))
                .Select(r => new { r.Code, r.Name })
                .ToListAsync();

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                BranchId = dto.BranchId ?? 0,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            if (existingCodes.Count > 0)
            {
                await _userManager.AddToRolesAsync(user, existingCodes.Select(x => x.Name!));
            }

            return user.UserName ?? string.Empty;
        }

        public async Task UpdateUserAsync(string userName, UpdateUserRequestDto dto)
        {
            var user = await _userManager.FindByNameAsync(userName)
                       ?? throw new Exception($"User '{userName}' not found.");

            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.Email != null) user.Email = dto.Email;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.BranchId.HasValue) user.BranchId = dto.BranchId.Value;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            if (dto.RoleCodes != null)
            {
                await AssignRolesAsync(userName, dto.RoleCodes);
            }
        }

        public async Task DeleteUserAsync(string userName, DeleteUserRequestDto dto)
        {
            var user = await _userManager.FindByNameAsync(userName)
                       ?? throw new Exception($"User '{userName}' not found.");
            // Soft-delete bằng lock + disable email xác thực (tuỳ chính sách)
            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        }
        #endregion
    }
}


