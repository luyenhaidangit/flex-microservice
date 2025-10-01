using Flex.AspNetIdentity.Api.Entities.Views;
using Flex.Infrastructure.EntityFrameworkCore.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
	public class UserRequestViewConfiguration : RequestViewBaseConfiguration<UserRequestView>
	{
		protected override void ConfigureView(EntityTypeBuilder<UserRequestView> builder)
		{
			// Map to database view
			builder.ToView("V_USER_REQUESTS");
			builder.HasNoKey();
		}

		protected override void ConfigureEntitySpecificProperties(EntityTypeBuilder<UserRequestView> builder)
		{
			// ===== User Specific Properties =====
			builder.Property(x => x.UserName)
				.HasColumnName("USER_NAME");

			builder.Property(x => x.FullName)
				.HasColumnName("FULL_NAME");

			builder.Property(x => x.Email)
				.HasColumnName("EMAIL");
		}
	}
}


