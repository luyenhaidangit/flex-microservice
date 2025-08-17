namespace Flex.Infrastructure.Redis
{
    public sealed class RedisOptions
    {
        public int KeepAliveSeconds { get; set; } = 60;     // giữ kết nối “ấm” để tránh idle bị cắt
        public int ConnectTimeoutMs { get; set; } = 8000;   // timeout bắt tay kết nối
        public int AsyncTimeoutMs { get; set; } = 8000;   // timeout cho lệnh async
        public int ConnectRetry { get; set; } = 3;      // số lần thử lại khi connect
        public bool AbortOnConnectFail { get; set; } = false; // tự phục hồi khi đứt
        public int ReconnectBaseMs { get; set; } = 5000; // thời gian cơ sở cho retry
        public string? InstanceName { get; set; } = string.Empty; // prefix key
    }
}
