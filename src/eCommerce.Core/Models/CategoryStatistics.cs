using System;

namespace eCommerce.Core.Models
{
    public class CategoryStatistics
    {
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageProductPrice { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal ReturnRate { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal YearOverYearGrowth { get; set; }
        public decimal MarketShare { get; set; }
        public decimal CustomerSatisfaction { get; set; }
        public decimal InventoryTurnover { get; set; }
        public int DaysOfInventory { get; set; }
    }
} 