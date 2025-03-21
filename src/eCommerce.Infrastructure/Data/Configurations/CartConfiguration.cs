using eCommerce.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Data.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                .IsRequired();

            builder.Property(c => c.SubTotal)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.Tax)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.ShippingCost)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.Discount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.Total)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt);

            builder.Property(c => c.CouponCode)
                .HasMaxLength(50);

            builder.Property(c => c.ShippingMethod)
                .HasMaxLength(50);

            builder.Property(c => c.PaymentMethod)
                .HasMaxLength(50);

            builder.Property(c => c.SessionId)
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.ShippingAddress)
                .WithMany()
                .HasForeignKey(c => c.ShippingAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Items)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => c.SessionId);
            builder.HasIndex(c => c.CreatedAt);
        }
    }

    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Quantity)
                .IsRequired();

            builder.Property(ci => ci.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(ci => ci.TotalPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(ci => ci.Discount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(ci => ci.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(ci => ci.ProductImage)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(ci => ci.CartId);
            builder.HasIndex(ci => ci.ProductId);
        }
    }
} 