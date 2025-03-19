using eCommerce.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Data.Configurations
{
    public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(v => v.Description)
                .HasMaxLength(1000);

            builder.Property(v => v.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(v => v.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(v => v.Address)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(v => v.VendorStatus)
                .IsRequired();

            builder.Property(v => v.CommissionRate)
                .HasPrecision(5, 2);

            builder.Property(v => v.AverageRating)
                .HasPrecision(3, 2);

            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.Property(v => v.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(v => v.User)
                .WithOne(u => u.Vendor)
                .HasForeignKey<Vendor>(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Products)
                .WithOne(p => p.Vendor)
                .HasForeignKey(p => p.VendorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Orders)
                .WithOne(o => o.Vendor)
                .HasForeignKey(o => o.Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(v => v.Reviews)
                .WithOne(r => r.Vendor)
                .HasForeignKey(r => r.VendorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 