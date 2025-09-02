using Flex.Shared.SeedWork.Workflow;

namespace Flex.AspNetIdentity.Api.Models.User
{
    /// <summary>
    /// DTO for paging pending user requests
    /// </summary>
    public class UserPendingPagingDto : RequestViewBase
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}