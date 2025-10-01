using Flex.Contracts.Domains.Interfaces;
using Flex.Notification.Api.Entities;
using Flex.Notification.Api.Entities.Views;
using Flex.Notification.Api.Persistence;
using Flex.Shared.SeedWork.Workflow;
using Microsoft.EntityFrameworkCore;

namespace Flex.Notification.Api.Repositories.Interfaces
{
	public interface INotificationTemplateRepository : IRepositoryBase<NotificationTemplate, Guid, NotificationDbContext>
    {
        DbSet<RequestBase<Guid>> GetNotificationTemplateRequestsDbSet();

        IQueryable<NotificationTemplateRequestView> GetNotificationTemplateRequests();
    }
}


