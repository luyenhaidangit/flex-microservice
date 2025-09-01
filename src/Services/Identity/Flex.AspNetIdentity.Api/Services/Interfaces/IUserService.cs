using Flex.AspNetIdentity.Api.Models.User;
using Flex.Shared.SeedWork;

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface IUserService
    {
        // Query
        Task<PagedResult<UserPagingDto>> GetUsersPagedAsync(GetUsersPagingRequest request, CancellationToken cancellationToken);
        Task<PagedResult<UserPendingPagingDto>> GetPendingUserRequestsPagedAsync(GetUserRequestsPagingRequest request, CancellationToken cancellationToken);
        Task<UserDetailDto> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);
        Task<List<UserChangeHistoryDto>> GetUserChangeHistoryAsync(string userName);

        // Commands on approved
        Task AssignRolesAsync(string userName, IEnumerable<string> roleCodes, CancellationToken ct = default);
        Task LockAsync(string userName, string? reason = null);
        Task UnlockAsync(string userName);
        Task<string> ResetPasswordAsync(string userName); // return reset token or correlation id

        // Pending style using RoleRequests pattern replaced by immediate create/update/delete with audit comment
        Task<string> CreateUserAsync(CreateUserRequestDto dto, string? comment = null);
        Task UpdateUserAsync(string userName, UpdateUserRequestDto dto);
        Task DeleteUserAsync(string userName, DeleteUserRequestDto dto);

        // User request management
        Task<UserRequestDetailDto> GetPendingUserRequestByIdAsync(long requestId);
        Task<UserRequestApprovalResultDto> ApprovePendingUserRequestAsync(long requestId, string? comment = null);
        Task<UserRequestApprovalResultDto> RejectPendingUserRequestAsync(long requestId, string? reason = null);
    }
}


