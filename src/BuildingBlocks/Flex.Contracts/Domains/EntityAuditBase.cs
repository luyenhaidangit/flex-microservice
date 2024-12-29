using Flex.Contracts.Domains.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Contracts.Domains
{
    public abstract class EntityAuditBase<T> : EntityBase<T>, IEntityAuditBase<T>
    {
        [Column("CREATEDDATE")]
        public DateTimeOffset CreatedDate { get; set; }

        [Column("LASTMODIFIEDDATE")]
        public DateTimeOffset? LastModifiedDate { get; set; }
    }
}
