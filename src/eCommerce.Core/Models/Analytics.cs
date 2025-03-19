using System;
using System.Collections.Generic;

namespace eCommerce.Core.Models
{
    public class Analytics
    {
        public required decimal TotalRevenue { get; set; }
        public required int TotalOrders { get; set; }
        public required int TotalCustomers { get; set; }
        public required decimal AverageOrderValue { get; set; }
        public required decimal ConversionRate { get; set; }
        public required decimal TotalCost { get; set; }
        public required decimal GrossProfit { get; set; }
        public required decimal NetProfit { get; set; }
        public required decimal ProfitMargin { get; set; }
        public required decimal AverageProductCost { get; set; }
        public required decimal InventoryTurnover { get; set; }
        public required int DaysOfInventory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockProductCount { get; set; }
        public decimal AverageProductPrice { get; set; }
        public decimal InventoryValue { get; set; }
        public ICollection<Product> TopSellingProducts { get; set; } = new List<Product>();
        public ICollection<Product> TopViewedProducts { get; set; } = new List<Product>();
        public ICollection<Product> LowStockProducts { get; set; } = new List<Product>();
        public ICollection<CategoryInventory> CategoryInventory { get; set; } = new List<CategoryInventory>();
        public ICollection<DailySales> DailySales { get; set; } = new List<DailySales>();
        public ICollection<CategorySales> CategorySales { get; set; } = new List<CategorySales>();
        public ICollection<PaymentMethodStats> PaymentMethodStats { get; set; } = new List<PaymentMethodStats>();
    }
} 