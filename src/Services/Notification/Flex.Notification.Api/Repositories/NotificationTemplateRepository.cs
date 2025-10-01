using Flex.Contracts.Domains.Interfaces;
using Flex.Infrastructure.Common.Repositories;
using Flex.Infrastructure.EntityFrameworkCore;
using Flex.Notification.Api.Entities;
using Flex.Notification.Api.Entities.Views;
using Flex.Notification.Api.Persistence;
using Flex.Notification.Api.Repositories.Interfaces;
using Flex.Shared.SeedWork.Workflow;
using Microsoft.EntityFrameworkCore;

namespace Flex.Notification.Api.Repositories
{
	public class NotificationTemplateRepository : RepositoryBase<NotificationTemplate, Guid, NotificationDbContext> ,INotificationTemplateRepository
	{
		private readonly NotificationDbContext _context;

		public NotificationTemplateRepository(NotificationDbContext context, IUnitOfWork<NotificationDbContext> unitOfWork) : base(context, unitOfWork)
		{
			_context = context;
		}

        public IQueryable<NotificationTemplateRequestView> GetNotificationTemplateRequests()
        {
            return _context.NotificationTemplateRequestViews.AsNoTracking();
        }

        public DbSet<RequestBase<Guid>> GetNotificationTemplateRequestsDbSet()
        {
            return _context.GetRequests<NotificationTemplate, Guid>();
        }
    }
}
