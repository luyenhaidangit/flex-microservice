using Flex.Infrastructure.EF;
using Flex.Infrastructure.Exceptions;
using Flex.Infrastructure.Workflow.DTOs;
using Flex.Notification.Api.Models.NotificationTemplate;
using Flex.Notification.Api.Repositories.Interfaces;
using Flex.Notification.Api.Services.Interfaces;
using Flex.Shared.Constants;
using Flex.Shared.SeedWork;
using Flex.Infrastructure.Workflow.Constants;
using Microsoft.EntityFrameworkCore;

namespace Flex.Notification.Api.Services
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly ILogger<NotificationTemplateService> _logger;
        private readonly INotificationTemplateRepository _notificationTemplateRepository;

        public NotificationTemplateService(
            ILogger<NotificationTemplateService> logger,
            INotificationTemplateRepository notificationTemplateRepository)
        {
            _logger = logger;
            _notificationTemplateRepository = notificationTemplateRepository;
        }

        #region Query

        /// <summary>
        /// Get all approved notification templates with pagination.
        /// </summary>
        public async Task<PagedResult<NotificationTemplatePagingDto>> GetNotificationTemplatesPagedAsync(GetNotificationTemplatesPagingRequest request, CancellationToken ct)
        {
            // ===== Process request parameters =====
            var keyword = request.Keyword?.Trim().ToLower();
            int pageIndex = request.PageIndexValue;
            int pageSize = request.PageSizeValue;

            // ===== Build query =====
            var query = _notificationTemplateRepository.FindAll().AsNoTracking()
                .WhereIf(!string.IsNullOrEmpty(keyword),
                    t => EF.Functions.Like((t.TemplateKey ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((t.Name ?? string.Empty).ToLower(), $"%{keyword}%")
                      || EF.Functions.Like((t.Subject ?? string.Empty).ToLower(), $"%{keyword}%"));

            // ===== Execute query =====
            var total = await query.CountAsync(ct);
            var raw = await query
                .OrderBy(t => t.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new { 
                    t.TemplateKey, 
                    t.Name, 
                    t.Channel, 
                    t.Format, 
                    t.Language, 
                    t.Subject, 
                    t.BodyHtml, 
                    t.BodyText, 
                    t.IsActive, 
                    t.VariablesSpecJson 
                })
                .ToListAsync(ct);

            // ===== Build result =====
            var items = raw.Select(t => new NotificationTemplatePagingDto
            {
                TemplateKey = t.TemplateKey,
                Name = t.Name,
                Channel = t.Channel,
                Format = t.Format,
                Language = t.Language,
                Subject = t.Subject,
                BodyHtml = t.BodyHtml,
                BodyText = t.BodyText,
                IsActive = t.IsActive,
                VariablesSpecJson = t.VariablesSpecJson
            }).ToList();

            // ===== Return result =====
            return PagedResult<NotificationTemplatePagingDto>.Create(pageIndex, pageSize, total, items);
        }

        /// <summary>
        /// Get notification template by ID.
        /// </summary>
        public async Task<NotificationTemplateDetailDto> GetNotificationTemplateByIdAsync(Guid templateId, CancellationToken ct)
        {
            // ===== Find template by ID =====
            var template = await _notificationTemplateRepository.FindAll().AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == templateId, ct);

            if (template == null)
            {
                throw new ValidationException(ErrorCode.TemplateNotFound);
            }

            // ===== Return result =====
            return new NotificationTemplateDetailDto
            {
                TemplateKey = template.TemplateKey,
                Name = template.Name,
                Channel = template.Channel,
                Format = template.Format,
                Language = template.Language,
                Subject = template.Subject,
                BodyHtml = template.BodyHtml,
                BodyText = template.BodyText,
                IsActive = template.IsActive,
                VariablesSpecJson = template.VariablesSpecJson
            };
        }

        /// <summary>
        /// Get notification template change history by template ID.
        /// </summary>
        public async Task<List<ChangeHistoryDto>> GetNotificationTemplateChangeHistoryAsync(Guid templateId, CancellationToken ct)
        {
            // ===== Find template by ID =====
            var template = await _notificationTemplateRepository.FindAll().AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == templateId, ct);

            if (template == null)
            {
                throw new ValidationException(ErrorCode.TemplateNotFound);
            }

            // ===== Get template change history =====
            var result = await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet()
                .AsNoTracking()
                .Where(r => r.EntityId == templateId)
                .OrderByDescending(r => r.RequestedDate)
                .Select(r => new ChangeHistoryDto
                {
                    Id = r.Id,
                    MakerBy = r.MakerId,
                    MakerTime = r.RequestedDate,
                    ApproverBy = r.CheckerId,
                    ApproverTime = r.ApproveDate,
                    Status = r.Status,
                    Description = r.Comments,
                    Changes = r.RequestedData
                })
                .ToListAsync(ct);

            return result;
        }

        /// <summary>
        /// Get all pending notification template requests with pagination.
        /// </summary>
        public async Task<PagedResult<NotificationTemplatePendingPagingDto>> GetPendingNotificationTemplateRequestsPagedAsync(GetNotificationTemplateRequestsPagingRequest request, CancellationToken ct)
        {
            return null;
            //// ===== Process request parameters =====
            //var keyword = request.Keyword?.Trim().ToLower();
            //var requestType = request.Type?.Trim().ToUpperInvariant();
            //int pageIndex = request.PageIndexValue;
            //int pageSize = request.PageSizeValue;

            //// ===== Build query =====
            //var pendingQuery = _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet()
            //    .WhereIf(!string.IsNullOrEmpty(keyword),
            //        r => EF.Functions.Like((r.RequestedData ?? string.Empty).ToLower(), $"%{keyword}%"))
            //    .Where(x => x.Status == RequestStatus.Unauthorised)
            //    .WhereIf(!string.IsNullOrEmpty(requestType) && requestType != RequestTypeConstant.All,
            //        r => r.Action == requestType)
            //    .AsNoTracking()
            //    .Select(r => new NotificationTemplatePendingPagingDto
            //    {
            //        RequestId = r.Id,
            //        EntityId = r.EntityId,
            //        Action = r.Action,
            //        RequestedBy = r.MakerId,
            //        RequestedDate = r.RequestedDate,
            //        Status = r.Status,
            //        TemplateKey = "", // Will be populated from template if exists
            //        Name = "", // Will be populated from template if exists
            //        Channel = "", // Will be populated from template if exists
            //        Subject = "" // Will be populated from template if exists
            //    });

            //// ===== Execute query =====
            //var total = await pendingQuery.CountAsync(ct);
            //var items = await pendingQuery
            //    .OrderByDescending(dto => dto.RequestedDate)
            //    .ThenBy(dto => dto.RequestId)
            //    .Skip((pageIndex - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToListAsync(ct);

            //// ===== Populate template information =====
            //foreach (var item in items)
            //{
            //    if (item.EntityId != Guid.Empty)
            //    {
            //        try
            //        {
            //            var template = await _notificationTemplateRepository.FindAll()
            //                .AsNoTracking()
            //                .FirstOrDefaultAsync(t => t.Id == item.EntityId, ct);
                        
            //            if (template != null)
            //            {
            //                item.TemplateKey = template.TemplateKey ?? string.Empty;
            //                item.Name = template.Name ?? string.Empty;
            //                item.Channel = template.Channel ?? string.Empty;
            //                item.Subject = template.Subject ?? string.Empty;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogWarning(ex, "Failed to get template information for EntityId: {EntityId}", item.EntityId);
            //        }
            //    }
            //}

            //// ===== Return result =====
            //return PagedResult<NotificationTemplatePendingPagingDto>.Create(pageIndex, pageSize, total, items);
        }

        #endregion
    }
}
