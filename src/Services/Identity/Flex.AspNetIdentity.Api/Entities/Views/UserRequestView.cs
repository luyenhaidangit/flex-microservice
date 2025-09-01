using Flex.Shared.SeedWork.Workflow;

namespace Flex.AspNetIdentity.Api.Entities.Views
{
	public class UserRequestView : RequestViewBase
    {
        public string UserName { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; } = default!;
    }
}

