using Flex.Infrastructure.Workflow.DTOs;
using Flex.Notification.Api.Models.NotificationTemplate;
using Flex.Shared.SeedWork;

namespace Flex.Notification.Api.Services.Interfaces
{
    public interface INotificationTemplateService
    {
        /// <summary>
        /// Get all approved notification templates with pagination.
        /// </summary>
        Task<PagedResult<NotificationTemplatePagingDto>> GetNotificationTemplatesPagedAsync(GetNotificationTemplatesPagingRequest request, CancellationToken ct);

        /// <summary>
        /// Get notification template by ID.
        /// </summary>
        Task<NotificationTemplateDetailDto> GetNotificationTemplateByIdAsync(Guid templateId, CancellationToken ct);

        /// <summary>
        /// Get notification template change history by template ID.
        /// </summary>
        Task<List<ChangeHistoryDto>> GetNotificationTemplateChangeHistoryAsync(Guid templateId, CancellationToken ct);

        /// <summary>
        /// Get all pending notification template requests with pagination.
        /// </summary>
        Task<PagedResult<NotificationTemplatePendingPagingDto>> GetPendingNotificationTemplateRequestsPagedAsync(GetNotificationTemplateRequestsPagingRequest request, CancellationToken ct);

        /// <summary>
        /// Get pending notification template request detail by request ID.
        /// </summary>
        Task<PendingRequestDtoBase<NotificationTemplateRequestDataDto>> GetPendingNotificationTemplateRequestDetailAsync(long requestId, CancellationToken ct);

        // Commands — maker (create pending requests)
        Task<long> CreateNotificationTemplateRequestAsync(CreateNotificationTemplateRequestDto dto, CancellationToken ct);
        Task<long> CreateUpdateNotificationTemplateRequestAsync(Guid templateId, UpdateNotificationTemplateRequestDto dto, CancellationToken ct);
        Task<long> CreateDeleteNotificationTemplateRequestAsync(Guid templateId, DeleteNotificationTemplateRequestDto dto, CancellationToken ct);

        // Commands — checker (approve/reject)
        Task<NotificationTemplateApprovalResultDto> ApprovePendingNotificationTemplateRequestAsync(long requestId, string? comment, CancellationToken ct);
        Task<NotificationTemplateApprovalResultDto> RejectPendingNotificationTemplateRequestAsync(long requestId, string? reason, CancellationToken ct);
    }
}
