using eCommerce.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Data.Configurations
{
    public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
    {
        public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
        {
            builder.HasKey(it => it.Id);

            builder.Property(it => it.Quantity)
                .IsRequired();

            builder.Property(it => it.UnitCost)
                .HasPrecision(18, 2);

            builder.Property(it => it.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(it => it.TotalCost)
                .HasPrecision(18, 2);

            builder.Property(it => it.RelatedTransferId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(it => it.RelatedDocumentNumber)
                .HasMaxLength(100);

            builder.Property(it => it.Notes)
                .HasMaxLength(500);

            builder.Property(it => it.CreatedAt)
                .IsRequired();

            builder.Property(it => it.ProcessedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(it => it.Inventory)
                .WithMany(i => i.Transactions)
                .HasForeignKey(it => it.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
} 