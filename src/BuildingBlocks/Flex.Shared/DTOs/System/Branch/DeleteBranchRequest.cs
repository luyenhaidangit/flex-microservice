namespace Flex.Shared.DTOs.System.Branch
{
    public class DeleteBranchRequest
    {
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
    }
}
