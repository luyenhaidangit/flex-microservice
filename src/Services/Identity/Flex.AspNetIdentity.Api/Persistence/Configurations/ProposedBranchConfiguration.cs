using Flex.AspNetIdentity.Api.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class ProposedBranchConfiguration : IEntityTypeConfiguration<ProposedBranch>
    {
        public void Configure(EntityTypeBuilder<ProposedBranch> builder)
        {
            builder.ToTable("V_PROPOSED_BRANCH");
            builder.HasNoKey();

            builder.Property(p => p.Code).HasColumnType("varchar2(255)");
            builder.Property(p => p.Name).HasColumnType("varchar2(256)");
            builder.Property(p => p.Action).HasColumnType("varchar2(20)");
            builder.Property(p => p.Status).HasColumnType("varchar2(10)");
            builder.Property(p => p.Description).HasColumnType("varchar2(1000)");
            builder.Property(p => p.CreatedBy).HasColumnType("varchar2(256)");
            builder.Property(p => p.UpdatedBy).HasColumnType("varchar2(256)");
        }
    }
}
