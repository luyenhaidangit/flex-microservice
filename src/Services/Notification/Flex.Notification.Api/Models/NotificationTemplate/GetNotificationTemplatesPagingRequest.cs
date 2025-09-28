using Flex.Shared.SeedWork;

namespace Flex.Notification.Api.Models.NotificationTemplate
{
    public class GetNotificationTemplatesPagingRequest : PagingRequest
    {
        public string? Keyword { get; set; }

        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
}
