using eCommerce.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Data.Configurations
{
    public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
    {
        public void Configure(EntityTypeBuilder<Inventory> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Quantity)
                .IsRequired();

            builder.Property(i => i.UnitCost)
                .HasPrecision(18, 2);

            builder.Property(i => i.ReorderPoint)
                .IsRequired();

            builder.Property(i => i.MaximumStockLevel)
                .IsRequired();

            builder.Property(i => i.Location)
                .HasMaxLength(100);

            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(i => i.Product)
                .WithOne(p => p.Inventory) // Assuming Product has an Inventory property
                .HasForeignKey<Inventory>(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete


            builder.HasMany(i => i.Transactions)
                .WithOne(it => it.Inventory)
                .HasForeignKey(it => it.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 