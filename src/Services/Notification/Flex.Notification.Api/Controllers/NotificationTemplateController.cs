using Flex.Infrastructure.EntityFrameworkCore;
using Flex.Notification.Api.Entities;
using Flex.Notification.Api.Persistence;
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

        public NotificationTemplateController(NotificationDbContext context)
        {
            _context = context;
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
    }
}
