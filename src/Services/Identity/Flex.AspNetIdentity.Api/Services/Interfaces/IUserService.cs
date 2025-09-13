using Flex.AspNetIdentity.Api.Models.User;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow;

namespace Flex.AspNetIdentity.Api.Services.Interfaces
{
    public interface IUserService
    {
        // Query
        Task<PagedResult<UserPagingDto>> GetUsersPagedAsync(GetUsersPagingRequest request, CancellationToken cancellationToken);
        Task<UserDetailDto> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);
        Task<List<UserChangeHistoryDto>> GetUserChangeHistoryAsync(string userName);
        Task<PagedResult<UserPendingPagingDto>> GetPendingUserRequestsPagedAsync(GetUserRequestsPagingRequest request, CancellationToken cancellationToken);
        Task<PendingRequestDtoBase<UserRequestDataDto>> GetPendingUserRequestByIdAsync(long requestId);
        // Command
        Task<long> CreateUserRequestAsync(CreateUserRequest request);
        Task<long> UpdateUserRequestAsync(UpdateUserRequest request);
        Task<long> DeleteUserRequestAsync(string userName);
        Task<UserRequestApprovalResultDto> ApprovePendingUserRequestAsync(long requestId, string? comment = null);
        Task<UserRequestApprovalResultDto> RejectPendingUserRequestAsync(long requestId, string? reason = null);
    }
}
