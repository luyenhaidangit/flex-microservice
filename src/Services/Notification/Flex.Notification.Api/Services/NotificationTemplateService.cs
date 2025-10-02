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
        private readonly ICurrentUserService _currentUserService;

        public NotificationTemplateService(
            ILogger<NotificationTemplateService> logger,
            INotificationTemplateRepository notificationTemplateRepository,
            ICurrentUserService currentUserService)
        {
            _logger = logger;
            _notificationTemplateRepository = notificationTemplateRepository;
            _currentUserService = currentUserService;
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
                    this.ConvertCreateNotificationTemplateRequestData(request, result);
                    break;
                case RequestType.Update:
                    this.ConvertUpdateNotificationTemplateRequestData(request, result);
                    break;
                case RequestType.Delete:
                    this.ConvertDeleteNotificationTemplateRequestData(request, result);
                    break;
                default:
                    throw new ValidationException(ErrorCode.InvalidRequestType);
            }

            return result;
        }

        #endregion

        #region Command — Maker

        public async Task<long> CreateNotificationTemplateRequestAsync(CreateNotificationTemplateRequestDto dto, CancellationToken ct)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // Uniqueness checks
            var exists = await _notificationTemplateRepository.FindAll()
                .AnyAsync(t => t.TemplateKey == dto.TemplateKey, ct);
            if (exists)
                throw new ValidationException(ErrorCode.Conflict);

            var pendingExists = await _notificationTemplateRepository.GetNotificationTemplateRequests()
                .AnyAsync(r => r.TemplateKey == dto.TemplateKey && r.Status == RequestStatus.Unauthorised, ct);
            if (pendingExists)
                throw new ValidationException(ErrorCode.Conflict);

            // Create request
            var maker = _currentUserService.GetCurrentUsername() ?? "anonymous";
            var requestedJson = System.Text.Json.JsonSerializer.Serialize(new NotificationTemplateRequestDataDto
            {
                TemplateKey = dto.TemplateKey,
                Name = dto.Name,
                Channel = dto.Channel,
                Format = dto.Format,
                Language = dto.Language,
                Subject = dto.Subject,
                BodyHtml = dto.BodyHtml,
                BodyText = dto.BodyText,
                IsActive = dto.IsActive,
                VariablesSpecJson = dto.VariablesSpecJson
            });

            var req = new RequestBase<Guid>
            {
                EntityId = Guid.Empty,
                Action = RequestType.Create,
                Status = RequestStatus.Unauthorised,
                MakerId = maker,
                RequestedDate = DateTime.UtcNow,
                RequestedData = requestedJson,
                Comments = string.IsNullOrWhiteSpace(dto.Comment) ? "Yêu cầu thêm mới mẫu thông báo." : dto.Comment
            };

            await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet().AddAsync(req, ct);
            await _notificationTemplateRepository.SaveChangesAsync();
            return req.Id;
        }

        public async Task<long> CreateUpdateNotificationTemplateRequestAsync(Guid templateId, UpdateNotificationTemplateRequestDto dto, CancellationToken ct)
        {
            // Find template
            var template = await _notificationTemplateRepository.FindAll().FirstOrDefaultAsync(t => t.Id == templateId, ct)
                           ?? throw new ValidationException(ErrorCode.TemplateNotFound);

            if (template.Status == RequestStatus.Unauthorised)
                throw new ValidationException(ErrorCode.Conflict);

            // Build new/old snapshot
            var oldData = new NotificationTemplateRequestDataDto
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
            var newData = new NotificationTemplateRequestDataDto
            {
                TemplateKey = template.TemplateKey, // immutable
                Name = dto.Name,
                Channel = dto.Channel,
                Format = dto.Format,
                Language = dto.Language,
                Subject = dto.Subject,
                BodyHtml = dto.BodyHtml,
                BodyText = dto.BodyText,
                IsActive = dto.IsActive,
                VariablesSpecJson = dto.VariablesSpecJson
            };

            var requestedJson = System.Text.Json.JsonSerializer.Serialize(new { newData, oldData });
            var maker = _currentUserService.GetCurrentUsername() ?? "anonymous";

            // Update status to lock
            template.Status = RequestStatus.Unauthorised;

            await using var tx = await _notificationTemplateRepository.BeginTransactionAsync();
            try
            {
                var req = new RequestBase<Guid>
                {
                    EntityId = template.Id,
                    Action = RequestType.Update,
                    Status = RequestStatus.Unauthorised,
                    MakerId = maker,
                    RequestedDate = DateTime.UtcNow,
                    RequestedData = requestedJson,
                    Comments = string.IsNullOrWhiteSpace(dto.Comment) ? "Yêu cầu cập nhật mẫu thông báo." : dto.Comment
                };

                await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet().AddAsync(req, ct);
                await _notificationTemplateRepository.SaveChangesAsync();

                await _notificationTemplateRepository.UpdateAsync(template);
                await tx.CommitAsync();
                return req.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<long> CreateDeleteNotificationTemplateRequestAsync(Guid templateId, DeleteNotificationTemplateRequestDto dto, CancellationToken ct)
        {
            // Find template
            var template = await _notificationTemplateRepository.FindAll().FirstOrDefaultAsync(t => t.Id == templateId, ct)
                           ?? throw new ValidationException(ErrorCode.TemplateNotFound);

            if (template.Status == RequestStatus.Unauthorised)
                throw new ValidationException(ErrorCode.Conflict);

            var oldData = new NotificationTemplateRequestDataDto
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
            var requestedJson = System.Text.Json.JsonSerializer.Serialize(oldData);

            var maker = _currentUserService.GetCurrentUsername() ?? "anonymous";
            template.Status = RequestStatus.Unauthorised;

            await using var tx = await _notificationTemplateRepository.BeginTransactionAsync();
            try
            {
                var req = new RequestBase<Guid>
                {
                    EntityId = template.Id,
                    Action = RequestType.Delete,
                    Status = RequestStatus.Unauthorised,
                    MakerId = maker,
                    RequestedDate = DateTime.UtcNow,
                    RequestedData = requestedJson,
                    Comments = string.IsNullOrWhiteSpace(dto.Comment) ? "Yêu cầu xóa mẫu thông báo." : dto.Comment
                };

                await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet().AddAsync(req, ct);
                await _notificationTemplateRepository.SaveChangesAsync();

                await _notificationTemplateRepository.UpdateAsync(template);
                await tx.CommitAsync();
                return req.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Command — Checker

        public async Task<NotificationTemplateApprovalResultDto> ApprovePendingNotificationTemplateRequestAsync(long requestId, string? comment, CancellationToken ct)
        {
            if (requestId <= 0) throw new ArgumentException("RequestId must be greater than 0.", nameof(requestId));

            var approver = _currentUserService.GetCurrentUsername() ?? "system";
            var request = await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatus.Unauthorised, ct)
                ?? throw new ValidationException(ErrorCode.RequestNotFound);

            // SoD: maker != checker
            if (!string.IsNullOrWhiteSpace(request.MakerId) && string.Equals(request.MakerId, approver, StringComparison.OrdinalIgnoreCase))
                throw new ValidationException(ErrorCode.Forbidden);

            await using var tx = await _notificationTemplateRepository.BeginTransactionAsync();
            try
            {
                Guid? createdId = null;

                switch (request.Action)
                {
                    case RequestType.Create:
                        createdId = await ApplyCreateAsync(request, ct);
                        comment = "Yêu cầu thêm mới mẫu thông báo.";
                        break;
                    case RequestType.Update:
                        await ApplyUpdateAsync(request, ct);
                        comment = "Yêu cầu cập nhật mẫu thông báo.";
                        break;
                    case RequestType.Delete:
                        await ApplyDeleteAsync(request, ct);
                        comment = "Yêu cầu xóa mẫu thông báo.";
                        break;
                    default:
                        throw new ValidationException(ErrorCode.InvalidRequestType);
                }

                // Update request status
                var reqEntity = await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet()
                    .FirstAsync(r => r.Id == request.Id, ct);
                reqEntity.Status = RequestStatus.Authorised;
                reqEntity.CheckerId = approver;
                reqEntity.ApproveDate = DateTime.UtcNow;
                reqEntity.Comments = comment ?? "Approved";
                await _notificationTemplateRepository.SaveChangesAsync();

                await tx.CommitAsync();

                return new NotificationTemplateApprovalResultDto
                {
                    RequestId = request.Id,
                    RequestType = request.Action,
                    Status = RequestStatus.Authorised,
                    ApprovedBy = approver,
                    ApprovedDate = DateTime.UtcNow,
                    Comment = comment,
                    CreatedTemplateId = createdId
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<NotificationTemplateApprovalResultDto> RejectPendingNotificationTemplateRequestAsync(long requestId, string? reason, CancellationToken ct)
        {
            if (requestId <= 0) throw new ArgumentException("RequestId must be greater than 0.", nameof(requestId));

            var rejecter = _currentUserService.GetCurrentUsername() ?? "system";
            var request = await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == requestId && r.Status == RequestStatus.Unauthorised, ct)
                ?? throw new ValidationException(ErrorCode.RequestNotFound);

            await using var tx = await _notificationTemplateRepository.BeginTransactionAsync();
            try
            {
                // Revert status for update/delete
                if ((request.Action == RequestType.Update || request.Action == RequestType.Delete) && request.EntityId != Guid.Empty)
                {
                    var entity = await _notificationTemplateRepository.FindAll().FirstOrDefaultAsync(t => t.Id == request.EntityId, ct);
                    if (entity != null)
                    {
                        entity.Status = RequestStatus.Authorised;
                        await _notificationTemplateRepository.UpdateAsync(entity);
                    }
                }

                // Update request status
                var reqEntity = await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet()
                    .FirstAsync(r => r.Id == request.Id, ct);
                reqEntity.Status = RequestStatus.Rejected;
                reqEntity.CheckerId = rejecter;
                reqEntity.ApproveDate = DateTime.UtcNow;
                reqEntity.Comments = reason ?? "Rejected";
                await _notificationTemplateRepository.SaveChangesAsync();

                await tx.CommitAsync();

                return new NotificationTemplateApprovalResultDto
                {
                    RequestId = request.Id,
                    RequestType = request.Action,
                    Status = RequestStatus.Rejected,
                    ApprovedBy = rejecter,
                    ApprovedDate = DateTime.UtcNow,
                    Comment = reason
                };
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private async Task<Guid> ApplyCreateAsync(RequestBase<Guid> request, CancellationToken ct)
        {
            var data = SafeParseNewData(request.RequestedData);
            if (data == null) throw new ValidationException(ErrorCode.InvalidRequestData);

            var entity = new Entities.NotificationTemplate
            {
                Id = Guid.NewGuid(),
                TemplateKey = data.TemplateKey,
                Name = data.Name,
                Channel = data.Channel,
                Format = data.Format,
                Language = data.Language,
                Subject = data.Subject,
                BodyHtml = data.BodyHtml,
                BodyText = data.BodyText,
                IsActive = data.IsActive,
                VariablesSpecJson = data.VariablesSpecJson,
                Status = RequestStatus.Authorised
            };
            await _notificationTemplateRepository.CreateAsync(entity);

            // Attach created Id back to request
            var reqEntity = await _notificationTemplateRepository.GetNotificationTemplateRequestsDbSet()
                .FirstAsync(r => r.Id == request.Id, ct);
            reqEntity.EntityId = entity.Id;
            await _notificationTemplateRepository.SaveChangesAsync();

            return entity.Id;
        }

        private async Task ApplyUpdateAsync(RequestBase<Guid> request, CancellationToken ct)
        {
            var data = SafeParseNewData(request.RequestedData);
            if (data == null) throw new ValidationException(ErrorCode.InvalidRequestData);

            var entity = await _notificationTemplateRepository.FindAll().FirstOrDefaultAsync(t => t.Id == request.EntityId, ct)
                         ?? throw new ValidationException(ErrorCode.TemplateNotFound);

            entity.Name = data.Name;
            entity.Channel = data.Channel;
            entity.Format = data.Format;
            entity.Language = data.Language;
            entity.Subject = data.Subject;
            entity.BodyHtml = data.BodyHtml;
            entity.BodyText = data.BodyText;
            entity.IsActive = data.IsActive;
            entity.VariablesSpecJson = data.VariablesSpecJson;
            entity.Status = RequestStatus.Authorised;

            await _notificationTemplateRepository.UpdateAsync(entity);
        }

        private async Task ApplyDeleteAsync(RequestBase<Guid> request, CancellationToken ct)
        {
            var entity = await _notificationTemplateRepository.FindAll().FirstOrDefaultAsync(t => t.Id == request.EntityId, ct)
                         ?? throw new ValidationException(ErrorCode.TemplateNotFound);
            await _notificationTemplateRepository.DeleteAsync(entity);
        }

        private NotificationTemplateRequestDataDto? SafeParseNewData(string rawJson)
        {
            try
            {
                // Support both direct object and wrapper { newData, oldData }
                // Try wrapper first
                var wrapper = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, object>>(rawJson);
                if (wrapper != null && wrapper.ContainsKey("newData"))
                {
                    var newDataJson = wrapper["newData"]?.ToString() ?? "{}";
                    return System.Text.Json.JsonSerializer.Deserialize<NotificationTemplateRequestDataDto>(newDataJson);
                }

                return System.Text.Json.JsonSerializer.Deserialize<NotificationTemplateRequestDataDto>(rawJson);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Convert create notification template request data.
        /// </summary>
        private void ConvertCreateNotificationTemplateRequestData(RequestBase<Guid> request, PendingRequestDtoBase<NotificationTemplateRequestDataDto> result)
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
        private void ConvertUpdateNotificationTemplateRequestData(RequestBase<Guid> request, PendingRequestDtoBase<NotificationTemplateRequestDataDto> result)
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
        private void ConvertDeleteNotificationTemplateRequestData(RequestBase<Guid> request, PendingRequestDtoBase<NotificationTemplateRequestDataDto> result)
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
