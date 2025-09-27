using Flex.Notification.Api.Models.Channel;

namespace Flex.Notification.Api.Services
{
    public class SmsService
    {
        public string ChannelName => "sms";
        public Task SendAsync(NotificationMessage message, CancellationToken ct = default)
            => Task.CompletedTask; // TODO: tích hợp Twilio/Nexmo...
    }
}
