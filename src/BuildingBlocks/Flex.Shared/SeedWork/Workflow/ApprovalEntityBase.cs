using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Shared.SeedWork.Workflow
{
    public abstract class ApprovalEntityBase<TKey> : EntityBase<TKey>
    {
        [Column("STATUS")]
        public required string Status { get; set; }
    }
}
