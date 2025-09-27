using Flex.Notification.Api.Helpers;
using Flex.Notification.Api.Models.Channel;
using Flex.Notification.Api.Models.Email;
using Flex.Notification.Api.Services.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Flex.Notification.Api.Services
{
    public class EmailService : INotificationService
    {
        public string ChannelName => "email";
        private readonly EmailOptions _opt;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailOptions> opt, ILogger<EmailService> logger)
        {
            _opt = opt.Value;
            _logger = logger;
        }

        public async Task SendAsync(NotificationMessage msg, CancellationToken ct = default)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_opt.FromName, _opt.FromAddress));
            email.To.Add(MailboxAddress.Parse(msg.Recipient));
            email.Subject = msg.SubjectOverride ?? GetSubject(msg);

            var htmlBody = await LoadAndRenderTemplateAsync(msg, ct);
            email.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_opt.SmtpHost, _opt.SmtpPort, _opt.UseStartTls, ct);
            await client.AuthenticateAsync(_opt.UserName, _opt.Password, ct);
            await client.SendAsync(email, ct);
            await client.DisconnectAsync(true, ct);
        }

        private static string GetSubject(NotificationMessage msg)
        => msg.TemplateKey switch
        {
            "AccountActivation" when msg.Language == "vi" => "Kích hoạt tài khoản của bạn",
            "AccountActivation" => "Activate your account",
            _ => "Flex Notification"
        };

        private static async Task<string> LoadAndRenderTemplateAsync(NotificationMessage msg, CancellationToken ct)
        {
            // Ưu tiên lấy từ DB template; nếu chưa có thì fallback file:
            var path = Path.Combine(AppContext.BaseDirectory, "Templates", "Email",
                $"{msg.TemplateKey}_{msg.Language}.html");

            var content = await File.ReadAllTextAsync(path, ct);
            return EmailHelpers.RenderTemplate(content, msg.Variables);
        }
    }
}
