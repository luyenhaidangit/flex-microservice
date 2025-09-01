using AutoMapper;
using Flex.AspNetIdentity.Api.Entities;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Flex.AspNetIdentity.Api.EventHandlers
{
    /// <summary>
    /// Handler xử lý event BranchCreated từ System service
    /// </summary>
    public class BranchCreatedEventHandler : IConsumer<BranchCreatedEvent>
    {
        private readonly IBranchCacheRepository _branchCacheRepository;
        private readonly IUnitOfWork<IdentityDbContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BranchCreatedEventHandler> _logger;

        public BranchCreatedEventHandler(
            IBranchCacheRepository branchCacheRepository,
            IUnitOfWork<IdentityDbContext> unitOfWork,
            IMapper mapper,
            ILogger<BranchCreatedEventHandler> logger)
        {
            _branchCacheRepository = branchCacheRepository ?? throw new ArgumentNullException(nameof(branchCacheRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<BranchCreatedEvent> context)
        {
            var message = context.Message;
            
            try
            {
                _logger.LogInformation("Processing BranchCreatedEvent for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);

                // Kiểm tra xem branch đã tồn tại chưa
                var existingBranch = await _branchCacheRepository.GetByCodeAsync(message.Code);
                if (existingBranch != null)
                {
                    _logger.LogWarning("Branch with code {Code} already exists in cache. Skipping creation.", message.Code);
                    return;
                }

                // Tạo branch cache mới
                var branchCache = new BranchCache
                {
                    Id = message.BranchId,
                    Code = message.Code,
                    Name = message.Name,
                    BranchType = message.BranchType,
                    IsActive = message.IsActive,
                    Description = message.Description,
                    LastSyncedAt = DateTime.UtcNow,
                    LastSyncedBy = message.CreatedBy
                };

                await _branchCacheRepository.AddAsync(branchCache);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully created branch cache for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing BranchCreatedEvent for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);
                throw; // Re-throw để MassTransit retry
            }
        }
    }
}
