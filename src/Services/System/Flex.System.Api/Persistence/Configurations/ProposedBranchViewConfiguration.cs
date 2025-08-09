using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.System.Api.Entities.Views;

namespace Flex.System.Api.Persistence.Configurations
{
    public class ProposedBranchViewConfiguration : IEntityTypeConfiguration<ProposedBranch>
    {
        public void Configure(EntityTypeBuilder<ProposedBranch> builder)
        {
            builder.ToView("V_PROPOSED_BRANCH");
            builder.HasNoKey();

            // Optional: Map column names (giữ tối thiểu để tránh ràng buộc không cần thiết)
            builder.Property(x => x.Id).HasColumnName("ID");
            builder.Property(x => x.EntityId).HasColumnName("ENTITY_ID");
            builder.Property(x => x.Status).HasColumnName("STATUS");
            builder.Property(x => x.Action).HasColumnName("ACTION");
            builder.Property(x => x.CreatedBy).HasColumnName("CREATED_BY");
            builder.Property(x => x.CreatedDate).HasColumnName("CREATED_DATE");
            builder.Property(x => x.CheckerId).HasColumnName("CHECKER_ID");
            builder.Property(x => x.ApproveDate).HasColumnName("APPROVE_DATE");
            builder.Property(x => x.UpdatedBy).HasColumnName("UPDATED_BY");
            builder.Property(x => x.UpdatedDate).HasColumnName("UPDATED_DATE");
            builder.Property(x => x.Code).HasColumnName("CODE");
            builder.Property(x => x.Name).HasColumnName("NAME");
            builder.Property(x => x.Description).HasColumnName("DESCRIPTION");
            builder.Property(x => x.IsActive).HasColumnName("IS_ACTIVE");
        }
    }
}
