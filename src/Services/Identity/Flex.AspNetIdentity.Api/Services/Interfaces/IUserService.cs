using Flex.AspNetIdentity.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> CreateUserAsync(CreateUserDto dto);
        Task<IdentityResult> UpdateUserAsync(long id, UpdateUserDto dto);
        Task<IdentityResult> ResetPasswordAsync(long id, string newPassword);
        Task<bool> LockUserAsync(long id);
        Task<bool> UnlockUserAsync(long id);
    }
}
