using Flex.Contracts.Domains.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Contracts.Domains
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        [Key]
        [Column("ID")]
        public TKey Id { get; set; }
    }
}
