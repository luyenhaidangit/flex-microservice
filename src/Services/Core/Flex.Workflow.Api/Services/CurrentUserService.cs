using System.Security.Claims;
using Flex.Workflow.Api.Services.Interfaces;

namespace Flex.Workflow.Api.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
        }

        public string? GetCurrentUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(Flex.Security.ClaimTypes.Sub);
        }
    }
}

