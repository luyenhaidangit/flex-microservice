using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Flex.System.Api.Entities;
using Flex.Shared.Constants.Common;

namespace Flex.System.Api.Data.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(x => x.Description)
                .HasMaxLength(500);
                
            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue(StatusConstant.Approved);
                
            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
                
            builder.Property(x => x.MemberCode)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(x => x.BranchType)
                .IsRequired()
                .HasMaxLength(50);
                
            builder.Property(x => x.Address)
                .HasMaxLength(500);
                
            builder.Property(x => x.ProvinceCode)
                .HasMaxLength(20);
                
            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(20);
                
            builder.Property(x => x.Email)
                .HasMaxLength(100);
                
            builder.Property(x => x.TaxCode)
                .HasMaxLength(50);
                
            builder.Property(x => x.LicenseNumber)
                .HasMaxLength(100);
                
            // Additional fields from business requirements
            builder.Property(x => x.ManagerName)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(x => x.EstablishmentDate);
            builder.Property(x => x.ClosedDate);
                
            builder.HasIndex(x => x.Code).IsUnique();
        }
    }
}
