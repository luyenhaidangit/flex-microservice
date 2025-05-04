namespace Flex.Shared.SeedWork.General
{
    public class ApproveOrRejectRequest<T>
    {
        public T Id { get; set; }
        public bool IsApprove { get; set; }
        public string ActionType { get; set; }
        public string? Comment { get; set; }
    }
}
