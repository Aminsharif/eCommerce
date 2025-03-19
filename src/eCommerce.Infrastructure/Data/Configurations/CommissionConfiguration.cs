using eCommerce.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Data.Configurations
{
    public class CommissionConfiguration : IEntityTypeConfiguration<Commission>
    {
        public void Configure(EntityTypeBuilder<Commission> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.OrderTotal)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.CommissionAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.VendorAmount)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.PaidAt)
                .IsRequired();

            // Relationships
            builder.HasOne(c => c.Order)
                .WithMany(o => o.Commissions)
                .HasForeignKey(c => c.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Vendor)
                .WithMany(v => v.Commissions)
                .HasForeignKey(c => c.VendorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 