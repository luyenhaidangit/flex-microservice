using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flex.Shared.Enums.General;

namespace Flex.Securities.Api.Entities
{
    [Table("ISSUERS")]
    public class CatalogIssuer : EntityAuditBase<long>
    {
        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string Name { get; set; }

        [Column(TypeName = "CLOB")]
        public string? Description { get; set; }

        #region Navigation
        public virtual ICollection<CatalogSecurities> Securities { get; set; } = new List<CatalogSecurities>();
        #endregion
    }
}
