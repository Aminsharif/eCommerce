using System;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Account
{
    public class AccountDashboardDto
    {
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int WishlistItemsCount { get; set; }
        public int PendingReviews { get; set; }
        public DateTime LastLoginDate { get; set; }
        public int UnreadNotifications { get; set; }
        public List<RecentOrderSummary> RecentOrders { get; set; } = new();
        public List<RecentActivity> RecentActivities { get; set; } = new();

        public class RecentOrderSummary
        {
            public string OrderNumber { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        public class RecentActivity
        {
            public DateTime ActivityDate { get; set; }
            public string ActivityType { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}