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
        public string? Description { get; set; }
    }
}
