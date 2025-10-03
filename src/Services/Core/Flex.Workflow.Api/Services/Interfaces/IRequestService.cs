using Flex.Shared.SeedWork;
using Flex.Workflow.Api.Models.Requests;

namespace Flex.Workflow.Api.Services.Interfaces
{
    public interface IRequestService
    {
        Task<long> CreateAsync(CreateApprovalRequestDto dto, CancellationToken ct = default);
        Task<PagedResult<PendingRequestListItemDto>> GetPendingPagedAsync(GetPendingRequestsPagingRequest request, CancellationToken ct = default);
        Task<RequestDetailDto> GetDetailAsync(long requestId, CancellationToken ct = default);
        Task ApproveAsync(long requestId, ApproveRequestDto dto, CancellationToken ct = default);
        Task RejectAsync(long requestId, RejectRequestDto dto, CancellationToken ct = default);
    }
}

