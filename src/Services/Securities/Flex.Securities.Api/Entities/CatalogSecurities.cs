using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flex.Contracts.Domains;
using Flex.Shared.Enums.Securities;
using Flex.Shared.Enums;

namespace Flex.Securities.Api.Entities
{
    [Table("SECURITIES")]
    public class CatalogSecurities : EntityAuditBase<long>
    {
        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public long Description { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(10)")]
        public EEntityStatus Status { get; set; }
    }
}
