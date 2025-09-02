using Flex.AspNetIdentity.Api.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
	public class UserRequestViewConfiguration : IEntityTypeConfiguration<UserRequestView>
	{
		public void Configure(EntityTypeBuilder<UserRequestView> builder)
		{
			// Map to database view
			builder.ToView("V_USER_REQUESTS");
			builder.HasNoKey();

			// Map properties to view columns
			builder.Property(x => x.RequestId)
				.HasColumnName("REQUEST_ID");

			builder.Property(x => x.EntityId)
				.HasColumnName("ENTITY_ID");

			builder.Property(x => x.Status)
				.HasColumnName("STATUS");

			builder.Property(x => x.Action)
				.HasColumnName("ACTION");

			builder.Property(x => x.RequestedBy)
				.HasColumnName("REQUESTED_BY");

			builder.Property(x => x.RequestedDate)
				.HasColumnName("REQUESTED_DATE");

			builder.Property(x => x.UserName)
				.HasColumnName("USER_NAME");

			builder.Property(x => x.FullName)
				.HasColumnName("FULL_NAME");

			builder.Property(x => x.Email)
				.HasColumnName("EMAIL");
		}
	}
}


