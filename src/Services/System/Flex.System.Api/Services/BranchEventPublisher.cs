using Flex.EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Flex.System.Api.Services
{
    /// <summary>
    /// Implementation của IBranchEventPublisher sử dụng MassTransit
    /// </summary>
    public class BranchEventPublisher : IBranchEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<BranchEventPublisher> _logger;

        public BranchEventPublisher(
            IPublishEndpoint publishEndpoint,
            ILogger<BranchEventPublisher> logger)
        {
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishBranchCreatedAsync(BranchCreatedEvent branchCreatedEvent)
        {
            try
            {
                await _publishEndpoint.Publish(branchCreatedEvent);
                _logger.LogInformation("Published BranchCreatedEvent for BranchId: {BranchId}, Code: {Code}", 
                    branchCreatedEvent.BranchId, branchCreatedEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish BranchCreatedEvent for BranchId: {BranchId}, Code: {Code}", 
                    branchCreatedEvent.BranchId, branchCreatedEvent.Code);
                throw;
            }
        }

        public async Task PublishBranchUpdatedAsync(BranchUpdatedEvent branchUpdatedEvent)
        {
            try
            {
                await _publishEndpoint.Publish(branchUpdatedEvent);
                _logger.LogInformation("Published BranchUpdatedEvent for BranchId: {BranchId}, Code: {Code}", 
                    branchUpdatedEvent.BranchId, branchUpdatedEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish BranchUpdatedEvent for BranchId: {BranchId}, Code: {Code}", 
                    branchUpdatedEvent.BranchId, branchUpdatedEvent.Code);
                throw;
            }
        }

        public async Task PublishBranchDeletedAsync(BranchDeletedEvent branchDeletedEvent)
        {
            try
            {
                await _publishEndpoint.Publish(branchDeletedEvent);
                _logger.LogInformation("Published BranchDeletedEvent for BranchId: {BranchId}, Code: {Code}", 
                    branchDeletedEvent.BranchId, branchDeletedEvent.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish BranchDeletedEvent for BranchId: {BranchId}, Code: {Code}", 
                    branchDeletedEvent.BranchId, branchDeletedEvent.Code);
                throw;
            }
        }
    }
}
