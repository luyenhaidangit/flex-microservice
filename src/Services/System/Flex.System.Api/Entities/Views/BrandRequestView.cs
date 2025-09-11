namespace Flex.System.Api.Entities.Views
{
    public class BrandRequestView
    {
        public long? Id { get; set; }
        public long? EntityId { get; set; }
        public string? Status { get; set; } = default!;
        public string? Action { get; set; } = default!;
        public string? CreatedBy { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public string? CheckerId { get; set; }
        public DateTime? ApproveDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Code { get; set; } = default!;
        public string? Name { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public string? IsActive { get; set; } = default!;
        public int? BranchType { get; set; }
    }
}
