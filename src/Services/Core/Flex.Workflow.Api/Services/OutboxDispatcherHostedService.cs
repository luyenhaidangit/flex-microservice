using Flex.Workflow.Api.Repositories.Interfaces;
using MassTransit;

namespace Flex.Workflow.Api.Services
{
    public class OutboxDispatcherHostedService : BackgroundService
    {
        private readonly ILogger<OutboxDispatcherHostedService> _logger;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IServiceProvider _sp;
        private readonly IConfiguration _configuration;

        public OutboxDispatcherHostedService(
            ILogger<OutboxDispatcherHostedService> logger,
            IOutboxRepository outboxRepository,
            IPublishEndpoint publishEndpoint,
            IServiceProvider sp,
            IConfiguration configuration)
        {
            _logger = logger;
            _outboxRepository = outboxRepository;
            _publishEndpoint = publishEndpoint;
            _sp = sp;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delayMs = _configuration.GetValue<int?>("Outbox:DispatchIntervalMs") ?? 2000;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Dispatch up to batch size
                    var items = _outboxRepository.FindByCondition(e => e.SentAt == null)
                        .OrderBy(e => e.CreatedAt)
                        .Take(50)
                        .ToList();

                    foreach (var ev in items)
                    {
                        // Publish generic event
                        await _publishEndpoint.Publish(new WorkflowEventMessage
                        {
                            Aggregate = ev.Aggregate,
                            AggregateId = ev.AggregateId,
                            EventType = ev.EventType,
                            Payload = ev.Payload,
                            CreatedAt = ev.CreatedAt
                        }, stoppingToken);

                        // Optional Webhooks
                        await TrySendWebhooksAsync(ev, stoppingToken);

                        ev.SentAt = DateTime.UtcNow;
                        await _outboxRepository.UpdateAsync(ev);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Outbox dispatch error");
                }

                await Task.Delay(delayMs, stoppingToken);
            }
        }

        private async Task TrySendWebhooksAsync(Flex.Workflow.Api.Entities.OutboxEvent ev, CancellationToken ct)
        {
            var endpoints = _configuration.GetSection("Webhooks:EventEndpoints").Get<string[]>() ?? Array.Empty<string>();
            if (endpoints.Length == 0) return;

            using var client = new HttpClient();
            var content = new StringContent(ev.Payload ?? string.Empty, System.Text.Encoding.UTF8, "application/json");
            foreach (var url in endpoints)
            {
                try { await client.PostAsync(url, content, ct); } catch { /* ignore */ }
            }
        }
    }

    public class WorkflowEventMessage
    {
        public string Aggregate { get; set; } = string.Empty;
        public string AggregateId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

