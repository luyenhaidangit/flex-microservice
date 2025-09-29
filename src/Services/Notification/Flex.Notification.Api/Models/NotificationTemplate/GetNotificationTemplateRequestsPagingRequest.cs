using Flex.Shared.SeedWork;

namespace Flex.Notification.Api.Models.NotificationTemplate
{
    /// <summary>
    /// DTO for paging notification template requests
    /// </summary>
    public class GetNotificationTemplateRequestsPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }
        public string? Type { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
