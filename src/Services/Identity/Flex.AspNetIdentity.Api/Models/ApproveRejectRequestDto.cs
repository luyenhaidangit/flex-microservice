namespace Flex.AspNetIdentity.Api.Models
{
    public class ApproveRoleRequestDto
    {
        public string? Comment { get; set; }
    }

    public class RejectRoleRequestDto
    {
        public string Reason { get; set; }
    }
}
