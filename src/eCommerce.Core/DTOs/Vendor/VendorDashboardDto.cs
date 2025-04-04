using System;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Vendor
{
    public class VendorDashboardDto
    {   
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal AverageRating { get; set; }
        public List<RecentOrder> RecentOrders { get; set; } = new();
        public List<ProductPerformance> TopPerformingProducts { get; set; } = new();
        public List<SalesByMonth> SalesHistory { get; set; } = new();

        public class RecentOrder
        {   
            public string OrderNumber { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        public class ProductPerformance
        {   
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int TotalSales { get; set; }
            public decimal Revenue { get; set; }
            public decimal Rating { get; set; }
            public int StockLevel { get; set; }
        }

        public class SalesByMonth
        {   
            public DateTime Month { get; set; }
            public decimal Revenue { get; set; }
            public int OrderCount { get; set; }
            public int ProductsSold { get; set; }
        }
    }
}