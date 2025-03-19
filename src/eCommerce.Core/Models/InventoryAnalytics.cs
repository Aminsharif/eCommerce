using System;
using System.Collections.Generic;

namespace eCommerce.Core.Models
{
    public class InventoryAnalytics
    {
        public int ProductId { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<InventoryTransaction> RecentTransactions { get; set; } = new();
        public decimal StockValue { get; set; }
        public decimal TurnoverRate { get; set; }
        public int StockDays { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public decimal AverageStockLevel { get; set; }
        public decimal StockoutRate { get; set; }
        public decimal HoldingCost { get; set; }
        public decimal OrderingCost { get; set; }
        public decimal TotalCost { get; set; }
        public int MinimumStockLevel { get; set; }
        public int MaximumStockLevel { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime LastModified { get; set; }
        public int TransactionCount { get; set; }
    }
} 