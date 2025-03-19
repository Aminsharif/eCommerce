using System;

namespace eCommerce.Core.Models
{
    public class CategorySales
    {
        public int Id { get; set; }
        public required string Category { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalItems { get; set; }
        public decimal AverageItemPrice { get; set; }
        public decimal TotalCost { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public required decimal ProfitMargin { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal RefundRate { get; set; }
        public int UniqueCustomers { get; set; }
        public decimal CustomerRetentionRate { get; set; }
        public decimal MarketShare { get; set; }
        public decimal YearOverYearGrowth { get; set; }
        public int StockLevel { get; set; }
        public decimal InventoryTurnover { get; set; }
        public int DaysOfInventory { get; set; }
    }
} 