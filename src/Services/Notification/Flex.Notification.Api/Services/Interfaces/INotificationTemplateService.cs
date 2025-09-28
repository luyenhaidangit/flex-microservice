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
    }
}
