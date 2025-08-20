namespace Flex.AspNetIdentity.Api.Models.User
{
    public class UpdateUserRequestDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public long? BranchId { get; set; }
        public bool? IsActive { get; set; }
        public List<string>? RoleCodes { get; set; }
        public string? Comment { get; set; }
    }
}


