using AutoMapper;
using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Models;
using Flex.AspNetIdentity.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Flex.AspNetIdentity.Api.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(UserManager<User> userManager, IMapper mapper, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IdentityResult> CreateUserAsync(CreateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);
            return await _userManager.CreateAsync(user, dto.Password);
        }

        public async Task<IdentityResult> UpdateUserAsync(long id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {Id} not found when updating", id);
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            if (dto.Email != null) user.Email = dto.Email;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.FullName != null) user.FullName = dto.FullName;
            if (dto.BranchId.HasValue) user.BranchId = dto.BranchId;

            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(long id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {Id} not found when resetting password", id);
                return IdentityResult.Failed(new IdentityError { Description = "User not found" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }

        public async Task<bool> LockUserAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {Id} not found when locking", id);
                return false;
            }

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return true;
        }

        public async Task<bool> UnlockUserAsync(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogWarning("User {Id} not found when unlocking", id);
                return false;
            }

            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
            return true;
        }
    }
}
