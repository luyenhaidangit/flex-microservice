using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Flex.AspNetIdentity.Api.Entities.Views
{
	[Keyless]
	[Table("V_PROPOSED_USER")]
	public class ProposedUser
	{
		[Column("ID")]
		public long? Id { get; set; }

		[Column("ENTITY_ID")]
		public long? EntityId { get; set; }

		[Column("USER_NAME")]
		public string? UserName { get; set; }

		[Column("FULL_NAME")]
		public string? FullName { get; set; }

		[Column("EMAIL")]
		public string? Email { get; set; }

		[Column("PHONE_NUMBER")]
		public string? PhoneNumber { get; set; }

		[Column("BRANCH_ID")]
		public long? BranchId { get; set; }

		[Column("ACTION")]
		public string? Action { get; set; }

		[Column("STATUS")]
		public string? Status { get; set; }

		[Column("CREATED_BY")]
		public string? CreatedBy { get; set; }

		[Column("CREATED_DATE")]
		public DateTime? CreatedDate { get; set; }

		[Column("UPDATED_BY")]
		public string? UpdatedBy { get; set; }

		[Column("UPDATED_DATE")]
		public DateTime? UpdatedDate { get; set; }

		[Column("CHECKER_ID")]
		public string? CheckerId { get; set; }

		[Column("APPROVE_DATE")]
		public DateTime? ApproveDate { get; set; }
	}
}


