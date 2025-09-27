using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Flex.Notification.Api.Persistence;
using Flex.Notification.Api.Entities;
using Flex.Shared.SeedWork;

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
