using System;

namespace eCommerce.Core.Models
{
    public class CategoryInventory
    {
        public required string Category { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public decimal TotalValue { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal AverageCost { get; set; }
        public decimal AverageOrderValue { get; set; }
        public required decimal ConversionRate { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal InventoryTurnover { get; set; }
        public int DaysOfInventory { get; set; }
    }
} 