using System;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Admin
{
    public class AdminDashboardDto
    {   
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalVendors { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockProducts { get; set; }
        public List<RecentOrder> RecentOrders { get; set; } = new();
        public List<TopProduct> TopProducts { get; set; } = new();
        public List<RevenueByMonth> RevenueHistory { get; set; } = new();

        public class RecentOrder
        {   
            public string OrderNumber { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        public class TopProduct
        {   
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int TotalSales { get; set; }
            public decimal Revenue { get; set; }
            public int StockLevel { get; set; }
        }

        public class RevenueByMonth
        {   
            public DateTime Month { get; set; }
            public decimal Revenue { get; set; }
            public int OrderCount { get; set; }
        }
    }
}