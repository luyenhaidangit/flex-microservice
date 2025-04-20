using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.System.Api.Entities
{
    [Table("BranchHistories")]
    public class BranchHistory : EntityAuditBase<long>
    {
        [Column("BRANCH_ID")]
        public long BranchId { get; set; }

        [Column("CHANGE_TYPE", TypeName = "VARCHAR2(50)")]
        public string ChangeType { get; set; } // Created, Updated, Closed

        [Column("OLD_DATA", TypeName = "CLOB")]
        public string? OldData { get; set; }

        [Column("NEW_DATA", TypeName = "CLOB")]
        public string? NewData { get; set; }

        [Column("COMMENT", TypeName = "NVARCHAR2(1000)")]
        public string? Comment { get; set; }
    }
}
