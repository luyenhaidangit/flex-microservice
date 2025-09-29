using Flex.Infrastructure.Exceptions;
using Flex.Infrastructure.Workflow.DTOs;
using Flex.Notification.Api.Models.NotificationTemplate;
using Flex.Notification.Api.Repositories.Interfaces;
using Flex.Notification.Api.Services.Interfaces;
using Flex.Shared.Constants;
using Flex.Shared.SeedWork;
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

        #endregion
    }
}
