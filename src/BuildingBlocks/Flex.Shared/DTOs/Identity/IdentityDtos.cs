using Flex.Shared.SeedWork;
using System.ComponentModel.DataAnnotations;

namespace Flex.Shared.DTOs.Identity
{
    #region Role
    public class CreateRoleRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public List<string>? Permissions { get; set; }
    }

    public class UpdateRoleRequest
    {
        [Required(ErrorMessage = "Id is required")]
        public string Id { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string? Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        public List<string>? Permissions { get; set; }
    }

    public class ManageRoleMemberRequest
    {
        [Required(ErrorMessage = "RoleId is required")]
        public string RoleId { get; set; } = string.Empty;

        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; } = string.Empty;
    }

    public class RoleListResponse : PagingRequest
    {
        public string? Name { get; set; }
        protected override Dictionary<string, string> OrderByMappings => new()
        {
        };
    }
    #endregion
}
