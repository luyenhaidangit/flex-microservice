namespace Flex.AspNetIdentity.Api.Entities.Views
{
    public class ProposedBranch
    {
        public int Id { get; set; }
        public long? EntityId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string? RequestType { get; set; }
        public string Status { get; set; }
        public bool? IsActive { get; set; } = true;
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
