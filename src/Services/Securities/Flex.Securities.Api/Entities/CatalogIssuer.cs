﻿using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Flex.Shared.Enums;

namespace Flex.Securities.Api.Entities
{
    [Table("ISSUERS")]
    public class CatalogIssuer : EntityAuditBase<long>
    {
        [Required]
        [Column(TypeName = "VARCHAR2(250)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "NUMBER(10)")]
        public EEntityStatus Status { get; set; }

        [Column(TypeName = "CLOB")]
        public string? Description { get; set; }

        #region Navigation
        public virtual ICollection<CatalogSecurity> Securities { get; set; } = new List<CatalogSecurity>();
        #endregion
    }
}