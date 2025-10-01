using Flex.Infrastructure.EF;
using Flex.Infrastructure.Exceptions;
using Flex.Infrastructure.Workflow.Constants;
using Flex.Infrastructure.Workflow.DTOs;
using Flex.Notification.Api.Models.NotificationTemplate;
using Flex.Notification.Api.Repositories.Interfaces;
using Flex.Notification.Api.Services.Interfaces;
using Flex.Shared.Constants;
using Flex.Shared.SeedWork;
using Flex.Shared.SeedWork.Workflow;
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
            // ===== Process request parameters =====
            var keyword = request.Keyword?.Trim().ToLower();
            var requestType = request.Type?.Trim().ToUpperInvariant();
            int pageIndex = request.PageIndexValue;
            int pageSize = request.PageSizeValue;

            // ===== Build query using view =====
            var pendingQuery = _notificationTemplateRepository.GetNotificationTemplateRequests()
                .WhereIf(!string.IsNullOrEmpty(keyword), 
                    r => EF.Functions.Like(r.TemplateKey.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(r.Name.ToLower(), $"%{keyword}%") ||
                         EF.Functions.Like(r.Subject.ToLower(), $"%{keyword}%"))
                .Where(x => x.Status == RequestStatus.Unauthorised)
                .WhereIf(!string.IsNullOrEmpty(requestType) && requestType != RequestType.All, 
                    r => r.Action == requestType)
                .AsNoTracking()
                .Select(r => new NotificationTemplatePendingPagingDto
                {
                    RequestId = r.RequestId,
                    EntityId = r.EntityId,
                    Action = r.Action,
                    RequestedBy = r.RequestedBy,
                    RequestedDate = r.RequestedDate,
                    Status = r.Status,
                    TemplateKey = r.TemplateKey,
                    Name = r.Name,
                    Channel = r.Channel,
                    Subject = r.Subject
                });

            // ===== Execute query =====
            var total = await pendingQuery.CountAsync(ct);
            var items = await pendingQuery
                .OrderByDescending(dto => dto.RequestedDate)
                .ThenBy(dto => dto.RequestId)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            // ===== Return result =====
            return PagedResult<NotificationTemplatePendingPagingDto>.Create(pageIndex, pageSize, total, items);
        }

        /// <summary>
        /// Get pending notification template request detail by request ID.
        /// </summary>
        public async Task<PendingRequestDtoBase<NotificationTemplateRequestDataDto>> GetPendingNotificationTemplateRequestDetailAsync(long requestId, CancellationToken ct)
        {
            // ===== Get request data =====
            var request = await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId, ct);

            if (request == null)
            {
                throw new ValidationException(ErrorCode.RequestNotFound);
            }

            // ===== Build base result =====
            var result = new PendingRequestDtoBase<NotificationTemplateRequestDataDto>
            {
                RequestId = request.Id.ToString(),
                Type = request.Action,
                CreatedBy = request.MakerId.ToString(),
                CreatedDate = request.RequestedDate.ToString("yyyy-MM-dd HH:mm:ss")
            };

            // ===== Process request data based on action type =====
            switch (request.Action)
            {
                case RequestType.Create:
                    await ConvertCreateNotificationTemplateRequestData(request, result);
                    break;
                case RequestType.Update:
                    await ConvertUpdateNotificationTemplateRequestData(request, result);
                    break;
                case RequestType.Delete:
                    await ConvertDeleteNotificationTemplateRequestData(request, result);
                    break;
                default:
                    throw new ValidationException(ErrorCode.InvalidRequestType);
            }

            return result;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Convert create notification template request data.
        /// </summary>
        private async Task ConvertCreateNotificationTemplateRequestData(RequestBase<Guid> request, PendingRequestDtoBase<NotificationTemplateRequestDataDto> result)
        {
            try
            {
                var newData = System.Text.Json.JsonSerializer.Deserialize<NotificationTemplateRequestDataDto>(request.RequestedData ?? "{}");
                result.NewData = newData;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize create notification template request data for request {RequestId}", request.Id);
                result.NewData = new NotificationTemplateRequestDataDto();
            }
        }

        /// <summary>
        /// Convert update notification template request data.
        /// </summary>
        private async Task ConvertUpdateNotificationTemplateRequestData(RequestBase<Guid> request, PendingRequestDtoBase<NotificationTemplateRequestDataDto> result)
        {
            try
            {
                // Parse the requested data JSON
                var requestData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(request.RequestedData ?? "{}");
                
                if (requestData != null && requestData.ContainsKey("newData") && requestData.ContainsKey("oldData"))
                {
                    var newDataJson = requestData["newData"].ToString();
                    var oldDataJson = requestData["oldData"].ToString();
                    
                    result.NewData = System.Text.Json.JsonSerializer.Deserialize<NotificationTemplateRequestDataDto>(newDataJson ?? "{}");
                    result.OldData = System.Text.Json.JsonSerializer.Deserialize<NotificationTemplateRequestDataDto>(oldDataJson ?? "{}");
                }
                else
                {
                    // Fallback: try to deserialize directly
                    var templateData = System.Text.Json.JsonSerializer.Deserialize<NotificationTemplateRequestDataDto>(request.RequestedData ?? "{}");
                    result.NewData = templateData;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize update notification template request data for request {RequestId}", request.Id);
                result.NewData = new NotificationTemplateRequestDataDto();
                result.OldData = new NotificationTemplateRequestDataDto();
            }
        }

        /// <summary>
        /// Convert delete notification template request data.
        /// </summary>
        private async Task ConvertDeleteNotificationTemplateRequestData(RequestBase<Guid> request, PendingRequestDtoBase<NotificationTemplateRequestDataDto> result)
        {
            try
            {
                var oldData = System.Text.Json.JsonSerializer.Deserialize<NotificationTemplateRequestDataDto>(request.RequestedData ?? "{}");
                result.OldData = oldData;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize delete notification template request data for request {RequestId}", request.Id);
                result.OldData = new NotificationTemplateRequestDataDto();
            }
        }

        #endregion
    }
}
