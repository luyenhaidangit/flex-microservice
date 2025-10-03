using Flex.AspNetIdentity.Api.Models.Auth;
using System.Security.Claims;

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult?> LoginAsync(LoginByUserNameRequest request, CancellationToken cancellationToken = default);
        Task<bool> LogoutAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
        Task<UserInfoResult?> GetCurrentUserInfoAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
    }
}
