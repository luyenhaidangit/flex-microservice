using AutoMapper;
using Flex.AspNetIdentity.Api.Repositories.Interfaces;
using Flex.EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Flex.AspNetIdentity.Api.EventHandlers
{
    /// <summary>
    /// Handler xử lý event BranchUpdated từ System service
    /// </summary>
    public class BranchUpdatedEventHandler : IConsumer<BranchUpdatedEvent>
    {
        private readonly IBranchCacheRepository _branchCacheRepository;
        private readonly IUnitOfWork<IdentityDbContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<BranchUpdatedEventHandler> _logger;

        public BranchUpdatedEventHandler(
            IBranchCacheRepository branchCacheRepository,
            IUnitOfWork<IdentityDbContext> unitOfWork,
            IMapper mapper,
            ILogger<BranchUpdatedEventHandler> logger)
        {
            _branchCacheRepository = branchCacheRepository ?? throw new ArgumentNullException(nameof(branchCacheRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<BranchUpdatedEvent> context)
        {
            var message = context.Message;
            
            try
            {
                _logger.LogInformation("Processing BranchUpdatedEvent for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);

                // Tìm branch cache hiện tại
                var existingBranch = await _branchCacheRepository.GetByIdAsync(message.BranchId);
                if (existingBranch == null)
                {
                    _logger.LogWarning("Branch with Id {BranchId} not found in cache. Creating new cache entry.", message.BranchId);
                    
                    // Tạo mới nếu không tìm thấy
                    var newBranchCache = new Flex.AspNetIdentity.Api.Entities.BranchCache
                    {
                        Id = message.BranchId,
                        Code = message.Code,
                        Name = message.Name,
                        BranchType = message.BranchType,
                        IsActive = message.IsActive,
                        Description = message.Description,
                        LastSyncedAt = DateTime.UtcNow,
                        LastSyncedBy = message.UpdatedBy
                    };

                    await _branchCacheRepository.AddAsync(newBranchCache);
                }
                else
                {
                    // Cập nhật thông tin
                    existingBranch.Code = message.Code;
                    existingBranch.Name = message.Name;
                    existingBranch.BranchType = message.BranchType;
                    existingBranch.IsActive = message.IsActive;
                    existingBranch.Description = message.Description;
                    existingBranch.LastSyncedAt = DateTime.UtcNow;
                    existingBranch.LastSyncedBy = message.UpdatedBy;

                    _branchCacheRepository.Update(existingBranch);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Successfully updated branch cache for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing BranchUpdatedEvent for BranchId: {BranchId}, Code: {Code}", 
                    message.BranchId, message.Code);
                throw; // Re-throw để MassTransit retry
            }
        }
    }
}
