using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.System.Api.Entities
{
    [Table("Configs")]
    public class Config : EntityAuditBase<long>
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
