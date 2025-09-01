using Flex.AspNetIdentity.Api.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
	public class UserRequestViewConfiguration : IEntityTypeConfiguration<UserRequestView>
	{
		public void Configure(EntityTypeBuilder<UserRequestView> builder)
		{
			builder.ToTable("V_USER_REQUESTS");
			builder.HasNoKey();
		}
	}
}


