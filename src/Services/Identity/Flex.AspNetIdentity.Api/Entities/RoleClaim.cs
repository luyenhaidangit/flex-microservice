using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.AspNetIdentity.Api.Entities
{
    public class RoleClaim : IdentityRoleClaim<long>
    {
        [Column("DESCRIPTION")]
        public string? Description { get; set; } = string.Empty;
    }
}