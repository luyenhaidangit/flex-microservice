namespace Flex.Shared.DTOs.System.Department
{
    public class DepartmentDto
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}
