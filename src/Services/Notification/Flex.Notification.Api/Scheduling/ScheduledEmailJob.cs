using Flex.Notification.Api.Entities;
using Flex.Notification.Api.Models.Channel;
using Flex.Notification.Api.Persistence;
using Flex.Notification.Api.Services.Interfaces;
using Quartz;

namespace Flex.Notification.Api.Scheduling
{
    public class NotificationScheduler : IJob
    {
        private readonly INotificationService _email;
        private readonly NotificationDbContext _db;

        public ScheduledEmailJob(IEnumerable<INotificationChannel> channels, NotificationDbContext db)
        { _email = channels.First(c => c.ChannelName == "email"); _db = db; }

        public async Task Execute(Quartz.IJobExecutionContext context)
        {
            var data = context.MergedJobDataMap; // lấy recipient, templateKey, vars...
            var recipient = data.GetString("recipient")!;
            var templateKey = data.GetString("templateKey")!;
            var lang = data.GetString("lang") ?? "en";
            var varsJson = data.GetString("vars") ?? "{}";
            var vars = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(varsJson)!;

            var log = NotificationLog.Pending("email", recipient, templateKey, Guid.NewGuid(), new { recipient, templateKey });
            log.Status = "Scheduled";
            _db.NotificationLogs.Add(log);
            await _db.SaveChangesAsync();

            await _email.SendAsync(new NotificationMessage
            {
                Recipient = recipient,
                TemplateKey = templateKey,
                Language = lang,
                Variables = vars
            });

            log.MarkSent();
            await _db.SaveChangesAsync();
        }
    }
}
