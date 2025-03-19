using System;
using System.Collections.Generic;

namespace eCommerce.Core.Models
{
    public class VendorAnalytics
    {
        public int VendorId { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public decimal CommissionEarned { get; set; }
        public decimal PendingCommissions { get; set; }
        
        // New metrics
        public decimal AverageOrderValue { get; set; }
        public decimal ConversionRate { get; set; }
        public int TotalCustomers { get; set; }
        public int RepeatCustomers { get; set; }
        public decimal CustomerRetentionRate { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal ReturnRate { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public decimal CustomerSatisfactionScore { get; set; }
        
        // Time-based metrics
        public decimal DailyRevenue { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        
        // Product performance
        public int TopSellingProductId { get; set; }
        public string TopSellingProductName { get; set; }
        public int TopSellingProductQuantity { get; set; }
        public decimal TopSellingProductRevenue { get; set; }
        public decimal CommissionsPaid { get; set; }
        public decimal CommissionRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

    }
} 