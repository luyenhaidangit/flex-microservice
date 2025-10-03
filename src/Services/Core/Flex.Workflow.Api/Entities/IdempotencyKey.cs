using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.Workflow.Api.Entities
{
    public class IdempotencyKey : EntityBase<long>
    {
        [Required]
        [Column("KEY", TypeName = "VARCHAR2(200)")]
        public string Key { get; set; } = string.Empty;

        [Required]
        [Column("FINGERPRINT", TypeName = "VARCHAR2(200)")]
        public string Fingerprint { get; set; } = string.Empty;

        [Required]
        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

