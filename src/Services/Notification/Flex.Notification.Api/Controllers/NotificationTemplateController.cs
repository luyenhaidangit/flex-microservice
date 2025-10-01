using Flex.Infrastructure.EntityFrameworkCore;
using Flex.Infrastructure.Workflow.DTOs;
using Flex.Notification.Api.Entities;
using Flex.Notification.Api.Models.NotificationTemplate;
using Flex.Notification.Api.Persistence;
using Flex.Notification.Api.Services.Interfaces;
using Flex.Shared.SeedWork;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Flex.Notification.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationTemplateController : ControllerBase
    {
        private readonly NotificationDbContext _context;
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateController(
            NotificationDbContext context,
            INotificationTemplateService notificationTemplateService)
        {
            _context = context;
            _notificationTemplateService = notificationTemplateService;
        }

        /// <summary>
        /// Get all notification templates
        /// </summary>
        /// <returns>List of notification templates</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationTemplate>>> GetAllNotificationTemplates()
        {
            try
            {
                var test = await _context.GetRequests<NotificationTemplate, Guid>().AsNoTracking().ToListAsync();

                var templates = await _context.NotificationTemplates
                    .OrderBy(t => t.Name)
                    .ToListAsync();

                return Ok(Result.Success(templates));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get pending notification template request detail by request ID.
        /// </summary>
        /// <param name="requestId">The request ID</param>
        /// <param name="ct">Cancellation token</param>
        /// <returns>Pending request detail</returns>
        [HttpGet("requests/pending/{requestId}")]
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
    }
}
