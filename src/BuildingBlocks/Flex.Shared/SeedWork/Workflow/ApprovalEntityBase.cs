using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Shared.SeedWork.Workflow
{
    public abstract class ApprovalEntityBase<TKey> : EntityBase<TKey>
    {
        [Column("STATUS")]
        public string Status { get; set; } = default!;
    }
}
