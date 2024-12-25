using Hangfire.Dashboard;

namespace Flex.EmailJob.Api.Filters
{
    public class AuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }
}
