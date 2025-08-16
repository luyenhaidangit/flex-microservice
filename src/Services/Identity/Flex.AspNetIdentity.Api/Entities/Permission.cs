using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Flex.Contracts.Domains;

namespace Flex.AspNetIdentity.Api.Entities
{
    public class Permission : EntityBase<long>
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? ParentPermissionId { get; set; }
        public bool IsAssignable { get; set; } = true;
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
        #region Navigation Properties
        public virtual Permission? ParentPermission { get; set; }
        public virtual ICollection<Permission> Children { get; set; } = new List<Permission>();
        #endregion
    }
}
