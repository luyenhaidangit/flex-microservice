using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.System.Api.Entities;

namespace Flex.System.Api.Persistence.Configurations
{
    public class BranchRequestConfiguration : IEntityTypeConfiguration<BranchRequest>
    {
        public void Configure(EntityTypeBuilder<BranchRequest> builder)
        {
            builder.ToTable("BRANCH_REQUESTS");

            // Key
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .HasColumnName("ID");

            // Entity (Branch) being requested
            builder.Property(x => x.EntityId)
                   .HasColumnName("ENTITY_ID");

            // Action: CREATE / UPDATE / DELETE
            builder.Property(x => x.Action)
                   .HasColumnName("ACTION")
                   .HasColumnType("VARCHAR2(20)")
                   .IsRequired();

            // Workflow status: PENDING / APPROVED / REJECTED
            builder.Property(x => x.Status)
                   .HasColumnName("STATUS")
                   .HasColumnType("VARCHAR2(10)")
                   .IsRequired();

            // JSON payload đề xuất thay đổi
            builder.Property(x => x.RequestedData)
                   .HasColumnName("REQUESTED_DATA")
                   .HasColumnType("CLOB")
                   .IsRequired();

            // Ghi chú của maker/checker
            builder.Property(x => x.Comments)
                   .HasColumnName("COMMENTS")
                   .HasColumnType("NVARCHAR2(500)");

            // Maker/Checker & thời gian
            builder.Property(x => x.MakerId)
                   .HasColumnName("REQUESTED_BY")
                   .HasColumnType("VARCHAR2(256)")
                   .IsRequired();

            builder.Property(x => x.RequestedDate)
                   .HasColumnName("REQUESTED_DATE")
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(x => x.CheckerId)
                   .HasColumnName("APPROVED_BY")
                   .HasColumnType("VARCHAR2(256)");

            builder.Property(x => x.ApproveDate)
                   .HasColumnName("APPROVE_DATE");

            // --- Indexes để tối ưu truy vấn màn hình duyệt ---
            builder.HasIndex(x => x.Status)
                   .HasDatabaseName("IX_BRANCH_REQ_STATUS");

            builder.HasIndex(x => new { x.EntityId, x.Status })
                   .HasDatabaseName("IX_BRANCH_REQ_ENTITY_STATUS");

            builder.HasIndex(x => x.RequestedDate)
                   .HasDatabaseName("IX_BRANCH_REQ_REQUESTED_DATE");
        }
    }
}