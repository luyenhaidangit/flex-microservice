using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models.User;
using Flex.AspNetIdentity.Api.Persistence;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Flex.Infrastructure.EF;
using Flex.Shared.SeedWork;
using Microsoft.EntityFrameworkCore;

namespace Flex.AspNetIdentity.Api.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IdentityDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IUserRequestRepository _userRequestRepository;
        private readonly ICurrentUserService _userService;

        public UserService(
            ILogger<UserService> logger,
            IdentityDbContext dbContext,
            ICurrentUserService userService,
            IUserRepository userRepository,
            IUserRequestRepository userRequestRepository)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userService = userService;
            _userRepository = userRepository;
            _userRequestRepository = userRequestRepository;
        }

        #region Query
        public async Task<PagedResult<UserPagingDto>> GetUsersPagedAsync(GetUsersPagingRequest request, CancellationToken ct)
        {
            var keyword = request.Keyword?.Trim().ToLowerInvariant();
            int pageIndex = request.PageIndexValue;
            int pageSize = request.PageSizeValue;

            var query = _userRepository.FindAll().AsNoTracking()
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    u => EF.Functions.Like((u.UserName ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((u.Email ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((u.FullName ?? string.Empty).ToLower(), $"%{keyword}%"))
                .WhereIf(request.BranchId.HasValue, u => u.BranchId == request.BranchId!.Value);

            var total = await query.CountAsync(ct);
            var raw = await query
                .OrderBy(u => u.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new { u.UserName, u.FullName, u.Email, u.PhoneNumber, u.BranchId, u.LockoutEnd })
                .ToListAsync(ct);

            var items = raw.Select(u => new UserPagingDto
            {
                UserName = u.UserName ?? string.Empty,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                BranchName = "",
                IsLocked = u.LockoutEnd.HasValue && u.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow,
                IsActive = true
            }).ToList();

            return PagedResult<UserPagingDto>.Create(pageIndex, pageSize, total, items);
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
                BranchName = "",
                IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd.Value.UtcDateTime > DateTime.UtcNow,
                IsActive = true
            };
        }

        public async Task<List<UserChangeHistoryDto>> GetUserChangeHistoryAsync(string userName)
        {
            // ===== Find user with code =====
            var user = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower());

            if (user == null)
            {
                throw new Exception($"User with username '{userName}' not exists.");
            }

            // ===== Get user histories by user Id =====
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
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower(), ct)
                       ?? throw new Exception($"User '{userName}' not found.");

            var codeSet = roleCodes?.Select(c => c?.Trim()).Where(c => !string.IsNullOrWhiteSpace(c)).ToHashSet(StringComparer.OrdinalIgnoreCase)
                          ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var roles = await _dbContext.Set<Role>().Where(r => codeSet.Contains(r.Code)).Select(r => new { r.Id }).ToListAsync(ct);

            var existing = await _dbContext.Set<UserRole>().Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToListAsync(ct);
            var target = roles.Select(r => r.Id).ToHashSet();

            var toRemove = existing.Where(id => !target.Contains(id)).ToList();
            var toAdd = target.Where(id => !existing.Contains(id)).ToList();

            if (toRemove.Count > 0)
            {
                var removeEntities = await _dbContext.Set<UserRole>().Where(ur => ur.UserId == user.Id && toRemove.Contains(ur.RoleId)).ToListAsync(ct);
                _dbContext.Set<UserRole>().RemoveRange(removeEntities);
            }
            if (toAdd.Count > 0)
            {
                var addEntities = toAdd.Select(id => new UserRole { UserId = user.Id, RoleId = id });
                await _dbContext.Set<UserRole>().AddRangeAsync(addEntities, ct);
            }

            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task LockAsync(string userName, string? reason = null)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower())
                       ?? throw new Exception($"User '{userName}' not found.");
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnlockAsync(string userName)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower())
                       ?? throw new Exception($"User '{userName}' not found.");
            user.LockoutEnd = null;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> ResetPasswordAsync(string userName)
        {
            var user = await _dbContext.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower())
                       ?? throw new Exception($"User '{userName}' not found.");
            // Không dùng Identity token provider nữa; tạm thời sinh mã ngẫu nhiên để gửi ngoài hệ thống
            return Guid.NewGuid().ToString("N");
        }
        #endregion

        #region Create/Update/Delete immediate with audit comment
        public async Task<string> CreateUserAsync(CreateUserRequestDto dto, string? comment = null)
        {
            // Validate role codes
            var roleIds = await _dbContext.Set<Role>()
                .Where(r => dto.RoleCodes.Contains(r.Code))
                .Select(r => r.Id)
                .ToListAsync();

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                BranchId = dto.BranchId ?? 0,
                FullName = dto.FullName
            };

            await _dbContext.Set<User>().AddAsync(user);
            await _dbContext.SaveChangesAsync();

            if (roleIds.Count > 0)
            {
                var maps = roleIds.Select(id => new UserRole { UserId = user.Id, RoleId = id });
                await _dbContext.Set<UserRole>().AddRangeAsync(maps);
                await _dbContext.SaveChangesAsync();
            }

            return user.UserName ?? string.Empty;
        }

        public async Task UpdateUserAsync(string userName, UpdateUserRequestDto dto)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower())
                       ?? throw new Exception($"User '{userName}' not found.");

            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.Email != null) user.Email = dto.Email;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.BranchId.HasValue) user.BranchId = dto.BranchId.Value;

            await _dbContext.SaveChangesAsync();

            if (dto.RoleCodes != null)
            {
                await AssignRolesAsync(userName, dto.RoleCodes);
            }
        }

        public async Task DeleteUserAsync(string userName, DeleteUserRequestDto dto)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(u => u.UserName!.ToLower() == userName.ToLower())
                       ?? throw new Exception($"User '{userName}' not found.");
            // Soft-delete bằng lock
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}


