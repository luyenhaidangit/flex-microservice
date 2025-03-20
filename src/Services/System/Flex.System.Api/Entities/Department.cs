using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Flex.System.Api.Entities
{
    public class Department : EntityAuditBase<long>
    {
        [Required]
        [Column("KEY", TypeName = "VARCHAR2(255)")]
        public string Key { get; set; }

        [Required]
        [Column("VALUE", TypeName = "CLOB")]
        public string Value { get; set; }

        [Column("DESCRIPTION", TypeName = "VARCHAR2(500)")]
        public string? Description { get; set; }
    }
}
