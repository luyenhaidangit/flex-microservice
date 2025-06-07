using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.AspNetIdentity.Api.Entities.Views
{
    [Keyless]
    [Table("V_PROPOSED_BRANCH")]
    public class ProposedBranch
    {
        [Column("ID")]
        public int Id { get; set; }

        [Column("ENTITY_ID")]
        public long? EntityId { get; set; }

        [Column("CODE")]
        public string Code { get; set; } = null!;

        [Column("NAME")]
        public string Name { get; set; } = null!;

        [Column("ACTION")]
        public string? Action { get; set; }

        [Column("STATUS")]
        public string Status { get; set; } = null!;

        [Column("IS_ACTIVE")]
        public bool? IsActive { get; set; } = true;

        [Column("DESCRIPTION")]
        public string? Description { get; set; }

        [Column("CREATED_BY")]
        public string? CreatedBy { get; set; }

        [Column("CREATED_DATE")]
        public DateTime? CreatedDate { get; set; }

        [Column("UPDATED_BY")]
        public string? UpdatedBy { get; set; }

        [Column("UPDATED_DATE")]
        public DateTime? UpdatedDate { get; set; }
    }
}
