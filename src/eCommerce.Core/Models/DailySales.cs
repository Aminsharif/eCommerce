using System;

namespace eCommerce.Core.Models
{
    public class DailySales
    {
        public int Id { get; set; }
        public required string Category { get; set; }
        public DateTime Date { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int ItemsSold { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal RefundRate { get; set; }
        public decimal ProfitMargin { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal TotalCost { get; set; }
        public decimal AverageItemPrice { get; set; }
        public decimal AverageItemCost { get; set; }
        public int UniqueCustomers { get; set; }
        public int NewCustomers { get; set; }
        public int ReturningCustomers { get; set; }
        public decimal CustomerRetentionRate { get; set; }
        public decimal CustomerAcquisitionCost { get; set; }
        public decimal CustomerLifetimeValue { get; set; }
    }
} 