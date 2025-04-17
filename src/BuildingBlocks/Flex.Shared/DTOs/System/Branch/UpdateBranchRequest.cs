namespace Flex.Shared.DTOs.System.Branch
{
    public class UpdateBranchRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Region { get; set; }
        public string? Status { get; set; }
        public DateTime? EstablishedDate { get; set; }
        public string? ManagerName { get; set; }
        public string? RequestedBy { get; set; }
    }
}
