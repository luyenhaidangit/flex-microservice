using Flex.Infrastructure.Workflow.DTOs;
using Flex.Notification.Api.Models.NotificationTemplate;
using Flex.Notification.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Flex.Notification.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationTemplateController : ControllerBase
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateController(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        /// <summary>
        /// Get approved notification templates (paged)
        /// </summary>
        [HttpGet("approved")]
        public async Task<IActionResult> GetApprovedTemplates([FromQuery] GetNotificationTemplatesPagingRequest request, CancellationToken ct)
        {
            var result = await _notificationTemplateService.GetNotificationTemplatesPagedAsync(request, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get approved template detail by ID.
        /// </summary>
        [HttpGet("approved/{templateId:guid}")]
        public async Task<IActionResult> GetApprovedTemplateById(Guid templateId, CancellationToken ct)
        {
            var result = await _notificationTemplateService.GetNotificationTemplateByIdAsync(templateId, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get approved template change history by ID.
        /// </summary>
        [HttpGet("approved/{templateId:guid}/history")]
        public async Task<IActionResult> GetApprovedTemplateHistory(Guid templateId, CancellationToken ct)
        {
            var result = await _notificationTemplateService.GetNotificationTemplateChangeHistoryAsync(templateId, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Create a new notification template request (maker)
        /// </summary>
        [HttpPost("requests/create")]
        public async Task<IActionResult> CreateTemplateRequest([FromBody] CreateNotificationTemplateRequestDto dto, CancellationToken ct)
        {
            var id = await _notificationTemplateService.CreateNotificationTemplateRequestAsync(dto, ct);
            return Ok(Result.Success(id));
        }

        /// <summary>
        /// Create update notification template request (maker)
        /// </summary>
        [HttpPost("approved/{templateId:guid}/update")]
        public async Task<IActionResult> CreateUpdateTemplateRequest(Guid templateId, [FromBody] UpdateNotificationTemplateRequestDto dto, CancellationToken ct)
        {
            var id = await _notificationTemplateService.CreateUpdateNotificationTemplateRequestAsync(templateId, dto, ct);
            return Ok(Result.Success(id));
        }

        /// <summary>
        /// Create delete notification template request (maker)
        /// </summary>
        [HttpPost("approved/{templateId:guid}/delete")]
        public async Task<IActionResult> CreateDeleteTemplateRequest(Guid templateId, [FromBody] DeleteNotificationTemplateRequestDto dto, CancellationToken ct)
        {
            var id = await _notificationTemplateService.CreateDeleteNotificationTemplateRequestAsync(templateId, dto, ct);
            return Ok(Result.Success(id));
        }

        /// <summary>
        /// Get all pending notification template requests with pagination.
        /// </summary>
        /// <param name="request">Paging and filter request</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Paged pending requests</returns>
        [HttpGet("pending")] 
        public async Task<IActionResult> GetPendingRequests([FromQuery] GetNotificationTemplateRequestsPagingRequest request, CancellationToken ct)
        {
            var result = await _notificationTemplateService.GetPendingNotificationTemplateRequestsPagedAsync(request, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Get pending notification template request detail by request ID.
        /// </summary>
        [HttpGet("pending/{requestId}")]
        public async Task<ActionResult<PendingRequestDtoBase<NotificationTemplateRequestDataDto>>> GetPendingNotificationTemplateRequestDetail(long requestId, CancellationToken ct)
        {
            try
            {
                var result = await _notificationTemplateService.GetPendingNotificationTemplateRequestDetailAsync(requestId, ct);
                return Ok(Result.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(Result.Failure(ex.Message));
            }
        }

        /// <summary>
        /// Approve pending notification template request (checker)
        /// </summary>
        [HttpPost("pending/{requestId}/approve")]
        public async Task<IActionResult> ApprovePendingRequest(long requestId, [FromBody] ApproveNotificationTemplateRequestDto? dto, CancellationToken ct)
        {
            var result = await _notificationTemplateService.ApprovePendingNotificationTemplateRequestAsync(requestId, dto?.Comment, ct);
            return Ok(Result.Success(result));
        }

        /// <summary>
        /// Reject pending notification template request (checker)
        /// </summary>
        [HttpPost("pending/{requestId}/reject")]
        public async Task<IActionResult> RejectPendingRequest(long requestId, [FromBody] RejectNotificationTemplateRequestDto? dto, CancellationToken ct)
        {
            var result = await _notificationTemplateService.RejectPendingNotificationTemplateRequestAsync(requestId, dto?.Reason, ct);
            return Ok(Result.Success(result));
        }
    }
}
