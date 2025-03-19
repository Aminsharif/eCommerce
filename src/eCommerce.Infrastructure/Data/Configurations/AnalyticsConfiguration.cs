using eCommerce.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eCommerce.Infrastructure.Data.Configurations
{
    public class AnalyticsConfiguration : IEntityTypeConfiguration<Analytics>
    {
        public void Configure(EntityTypeBuilder<Analytics> builder)
        {
            builder.HasNoKey();

            builder.Property(a => a.TotalCost)
                .HasPrecision(18, 2);

            builder.Property(a => a.TotalRevenue)
                .HasPrecision(18, 2);

            builder.Property(a => a.GrossProfit)
                .HasPrecision(18, 2);

            builder.Property(a => a.NetProfit)
                .HasPrecision(18, 2);

            builder.Property(a => a.ProfitMargin)
                .HasPrecision(5, 2);

            builder.Property(a => a.AverageOrderValue)
                .HasPrecision(18, 2);

            builder.Property(a => a.ConversionRate)
                .HasPrecision(5, 2);

            builder.Property(a => a.InventoryValue)
                .HasPrecision(18, 2);

            builder.Property(a => a.InventoryTurnover)
                .HasPrecision(5, 2);

            builder.Property(a => a.AverageProductCost)
                .HasPrecision(18, 2);

            builder.Property(a => a.AverageProductPrice)
                .HasPrecision(18, 2);

            builder.ToView("vw_Analytics");
        }
    }
} 