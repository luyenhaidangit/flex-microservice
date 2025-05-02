namespace Flex.Shared.DTOs.System.Branch
{
    public class BranchPagingDto
    {
        public long? Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? PendingAction { get; set; }
        public DateTime? RequestedDate { get; set; }

        public BranchPagingDto(
        long? id,
        string code,
        string name,
        string? address,
        string? pendingAction,
        DateTime? requestedDate)
        {
            Id = id;
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address;
            PendingAction = pendingAction;
            RequestedDate = requestedDate;
        }
    }
}
