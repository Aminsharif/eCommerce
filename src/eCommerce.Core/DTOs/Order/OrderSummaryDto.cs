using System;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Order
{
    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string ShippingMethod { get; set; } = string.Empty;
        public string TrackingNumber { get; set; } = string.Empty;
        public DateTime? EstimatedDeliveryDate { get; set; }
        public List<OrderSummaryItemDto> Items { get; set; } = new List<OrderSummaryItemDto>();
    }

    public class OrderSummaryItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}