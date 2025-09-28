using Flex.Contracts.Domains.Interfaces;
using Flex.Notification.Api.Entities;
using Flex.Notification.Api.Persistence;

namespace Flex.AspNetIdentity.Api.Repositories.Interfaces
{
	public interface INotificationTemplateRepository : IRepositoryBase<NotificationTemplate, Guid, NotificationDbContext>
    {
	}
}


