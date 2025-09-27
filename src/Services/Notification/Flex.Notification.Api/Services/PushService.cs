using Flex.Notification.Api.Models.Channel;

namespace Flex.Notification.Api.Services
{
    public class PushService
    {
        public string ChannelName => "push";
        public Task SendAsync(NotificationMessage message, CancellationToken ct = default)
            => Task.CompletedTask; // TODO: tích hợp FCM/APNS...
    }
}
