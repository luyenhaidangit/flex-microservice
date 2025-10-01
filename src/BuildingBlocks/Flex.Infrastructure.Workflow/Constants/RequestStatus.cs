namespace Flex.Infrastructure.Workflow.Constants
{
    public static class RequestStatus
    {
        public const string Unauthorised = "UNA"; // Unauthorised (chờ duyệt)
        public const string Authorised = "AUT"; // Authorised (đã duyệt)
        public const string Rejected = "REJ"; // Rejected
        public const string Cancelled = "CAN"; // Cancelled
        public const string Hold = "HLD"; // Hold (tạm hoãn)
        public const string Expired = "EXP"; // Expired (hết hạn)
    }
}
