using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using Flex.Shared.Enums.General;

namespace Flex.Securities.Api.Entities
{
    [Table("ISSUERS")]
    public class CatalogIssuer : EntityAuditBase<long>
    {
        [Column(TypeName = "VARCHAR2(250)")]
        public string? Name { get; set; }

        [Column(TypeName = "VARCHAR2(50)")]
        public string? Code { get; set; }

        [Column(TypeName = "CLOB")]
        public string? Description { get; set; }

        [Column(TypeName = "NUMBER(10)")]
        public EProcessStatus? ProcessStatus { get; set; }

        #region Navigation
        public virtual ICollection<CatalogSecurities> Securities { get; set; } = new List<CatalogSecurities>();
        #endregion
    }
}
