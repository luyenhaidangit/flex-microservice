using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Repositories;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Flex.AspNetIdentity.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly IUserRequestRepository _userRequestRepository;
        private readonly ICurrentUserService _userService;

        public UserService(
            ILogger<UserService> logger,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            ICurrentUserService userService,
            IUserRepository userRepository,
            IUserRequestRepository userRequestRepository)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _userRepository = userRepository;
            _userRequestRepository = userRequestRepository;
        }

        #region Query
        public async Task<PagedResult<UserPagingDto>> GetApprovedUsersPagedAsync(GetUsersPagingRequest request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetApprovedUsersPagedAsync(request, cancellationToken);
            return result;
        }

        public async Task<UserDetailDto> GetUserByUserNameAsync(string userName, CancellationToken ct)
        {
            var user = await _userRepository.FindAll().AsNoTracking().FirstOrDefaultAsync(u => u.UserName.ToLowerInvariant() == userName.ToLowerInvariant(), ct);

            if(user == null)
            {
                throw new Exception($"User '{userName}' not found.");
            };

            return new UserDetailDto
            {
                UserName = user.UserName ?? string.Empty,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                BranchId = user.BranchId,
                IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow,
                IsActive = true,
                //Roles = roles.ToList()
            };
        }

        public async Task<List<UserChangeHistoryDto>> GetUserChangeHistoryAsync(string userName)
        {
            // ===== Find user with code =====
            var user = _userRepository.ExistsByUserNameAsync(userName);

            if (user == null)
            {
                throw new Exception($"User with username '{userName}' not exists.");
            }

            // ===== Get user histories by role Id =====
            var userId = user.Id;

            var requests = await _userRequestRepository.FindAll()
                .Where(r => r.EntityId == userId).AsNoTracking()
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

            var historyItems = requests.Select((req, idx) => new UserChangeHistoryDto
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


