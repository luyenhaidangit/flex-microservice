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
    }
}
