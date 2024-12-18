using Flex.Contracts.Domains.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Flex.Contracts.Domains
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        [Key]
        public TKey Id { get; set; }
    }
}
