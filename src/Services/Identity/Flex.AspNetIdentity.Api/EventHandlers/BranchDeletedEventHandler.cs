using AutoMapper;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Flex.AspNetIdentity.Api.EventHandlers
{
    /// <summary>
    /// Handler xử lý event BranchDeleted từ System service
    /// </summary>
    public class BranchDeletedEventHandler : IConsumer<BranchDeletedEvent>
    {
        private readonly IBranchCacheRepository _branchCacheRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BranchDeletedEventHandler> _logger;

        public BranchDeletedEventHandler(
            IBranchCacheRepository branchCacheRepository,
            IMapper mapper,
            ILogger<BranchDeletedEventHandler> logger)
        {
            _branchCacheRepository = branchCacheRepository ?? throw new ArgumentNullException(nameof(branchCacheRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<BranchDeletedEvent> context)
        {
            var message = context.Message;
            
            try
            {
                _logger.LogInformation("Processing BranchDeletedEvent for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);

                // Tìm branch cache hiện tại
                var existingBranch = await _branchCacheRepository.GetByIdAsync(message.BranchId);
                if (existingBranch == null)
                {
                    _logger.LogWarning("Branch with Id {BranchId} not found in cache. Nothing to delete.", message.BranchId);
                    return;
                }

                // Xóa branch cache
                await _branchCacheRepository.DeleteAsync(existingBranch);
                await _branchCacheRepository.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted branch cache for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing BranchDeletedEvent for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);
                throw; // Re-throw để MassTransit retry
            }
        }
    }
}
