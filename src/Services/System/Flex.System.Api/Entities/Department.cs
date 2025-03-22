using Flex.Contracts.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.System.Api.Entities
{
    [Table("Departments")]
    public class Department : EntityAuditBase<long>
    {
        [Required]
        [Column("NAME", TypeName = "VARCHAR2(400)")]
        public string Name { get; set; } // Tên phòng ban/chi nhánh

        [Column("ADDRESS", TypeName = "VARCHAR2(100)")]
        public string? Address { get; set; } // Địa chỉ hoặc tên người quản lý

        [Column("DESCRIPTION", TypeName = "CLOB")]
        public string? Description { get; set; } // Mô tả

        [Required]
        [Column("STATUS", TypeName = "CHAR(1)")]
        public string Status { get; set; } // Trạng thái (A: Active, E: Expired,...)
    }
}
