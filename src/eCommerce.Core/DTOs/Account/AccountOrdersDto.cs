using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Account
{
    public class AccountOrdersDto
    {
        public List<OrderItem> Orders { get; set; } = new();
        public int TotalOrders { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public class OrderItem
        {
            public string OrderNumber { get; set; } = string.Empty;
            public DateTime OrderDate { get; set; }
            public decimal TotalAmount { get; set; }
            public string Status { get; set; } = string.Empty;
            public string PaymentMethod { get; set; } = string.Empty;
            public string ShippingMethod { get; set; } = string.Empty;
            public bool HasReviews { get; set; }
            public List<OrderItemDetail> Items { get; set; } = new();
        }

        public class OrderItemDetail
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string ProductImage { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Subtotal { get; set; }
        }
    }
}