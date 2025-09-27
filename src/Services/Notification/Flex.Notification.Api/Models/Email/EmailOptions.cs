namespace Flex.Notification.Api.Models.Email
{
    public class EmailOptions
    {
        public string SmtpHost { get; set; } = default!;
        public int SmtpPort { get; set; }
        public bool UseStartTls { get; set; } = true;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string FromAddress { get; set; } = default!;
        public string FromName { get; set; } = "Flex";
        public int MaxParallelSends { get; set; } = 10;
    }
}
