using Flex.Notification.Api.Models.Channel;

namespace Flex.Notification.Api.Services.Interfaces
{
    public interface INotificationService
    {
        string ChannelName { get; } // "email" | "sms" | "push"
        Task SendAsync(NotificationMessage message, CancellationToken ct = default);
    }
}
