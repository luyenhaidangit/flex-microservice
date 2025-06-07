using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.AspNetIdentity.Api.Entities;

namespace Flex.AspNetIdentity.Api.Persistence.Configurations
{
    public class RoleRequestConfiguration : IEntityTypeConfiguration<RoleRequest>
    {
        public void Configure(EntityTypeBuilder<RoleRequest> builder)
        {
            builder.ToTable("ROLE_REQUESTS");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasColumnName("ID");

            builder.Property(x => x.EntityId)
                .HasColumnName("ENTITY_ID");

            builder.Property(x => x.Action)
                .HasColumnName("ACTION")
                .HasColumnType("varchar2(20)");

            builder.Property(x => x.Status)
                .HasColumnName("STATUS")
                .HasColumnType("varchar2(10)");

            builder.Property(x => x.RequestedData)
                .HasColumnName("REQUESTED_DATA")
                .HasColumnType("clob");

            builder.Property(x => x.Comments)
                .HasColumnName("COMMENTS")
                .HasColumnType("nvarchar2(500)");

            builder.Property(x => x.MakerId)
                .HasColumnName("MAKER_ID")
                .HasColumnType("varchar2(256)");

            builder.Property(x => x.RequestedDate)
                .HasColumnName("REQUESTED_DATE")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.CheckerId)
                .HasColumnName("CHECKER_ID")
                .HasColumnType("varchar2(256)");

            builder.Property(x => x.ApproveDate)
                .HasColumnName("APPROVE_DATE");
        }
    }
}
