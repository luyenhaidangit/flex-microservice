using Flex.Contracts.Events.Users;
using Flex.Notification.Api.Entities;
using Flex.Notification.Api.Models.Channel;
using Flex.Notification.Api.Persistence;
using Flex.Notification.Api.Services.Interfaces;
using MassTransit;
using MassTransit.SqlTransport;

namespace Flex.Notification.Api.Consumers
{
    public class UserCreatedApprovedConsumer : IConsumer<UserCreatedApprovedEvent>
    {
        private readonly IEnumerable<INotificationService> _channels;
        private readonly NotificationDbContext _db;
        private readonly ILogger<UserCreatedApprovedConsumer> _logger;

        public UserCreatedApprovedConsumer(IEnumerable<INotificationService> channels,
                                    NotificationDbContext db,
                                    ILogger<UserCreatedApprovedConsumer> logger)
        {
            _channels = channels;
            _db = db;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedApprovedEvent> context)
        {
            var evt = context.Message;

            // log PENDING
            var log = NotificationLog.Pending(
                channel: "email",
                recipient: evt.Email,
                templateKey: "AccountActivation",
                correlationId: context.CorrelationId ?? Guid.NewGuid(),
                payload: new { evt.UserId, evt.FullName, evt.ActivationLink, evt.Language });

            _db.NotificationLogs.Add(log);
            await _db.SaveChangesAsync(context.CancellationToken);

            // chọn channel
            var emailChannel = _channels.First(c => c.ChannelName == "email");

            try
            {
                await emailChannel.SendAsync(new NotificationMessage
                {
                    Recipient = evt.Email,
                    TemplateKey = "AccountActivation",
                    Language = string.IsNullOrWhiteSpace(evt.Language) ? "en" : evt.Language,
                    Variables = new()
                    {
                        ["Name"] = evt.FullName,
                        ["ActivationLink"] = evt.ActivationLink
                    }
                }, context.CancellationToken);

                log.MarkSent();
                await _db.SaveChangesAsync(context.CancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send activation email failed for {Email}", evt.Email);
                log.MarkFailed(ex.Message);
                await _db.SaveChangesAsync(context.CancellationToken);
                throw; // để MassTransit retry theo policy
            }
        }
    }
}
