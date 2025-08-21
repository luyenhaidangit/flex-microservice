using Flex.AspNetIdentity.Api.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
	public class ProposedUserConfiguration : IEntityTypeConfiguration<ProposedUser>
	{
		public void Configure(EntityTypeBuilder<ProposedUser> builder)
		{
			builder.ToTable("V_PROPOSED_USER");
			builder.HasNoKey();

			builder.Property(p => p.UserName).HasColumnType("varchar2(256)");
			builder.Property(p => p.FullName).HasColumnType("varchar2(256)");
			builder.Property(p => p.Email).HasColumnType("varchar2(256)");
			builder.Property(p => p.PhoneNumber).HasColumnType("varchar2(50)");
			builder.Property(p => p.Action).HasColumnType("varchar2(20)");
			builder.Property(p => p.Status).HasColumnType("varchar2(10)");
			builder.Property(p => p.CreatedBy).HasColumnType("varchar2(256)");
			builder.Property(p => p.UpdatedBy).HasColumnType("varchar2(256)");
		}
	}
}


