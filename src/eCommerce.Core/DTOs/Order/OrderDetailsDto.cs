using System;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Order
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
        public string ShippingMethod { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string? PaymentDetails { get; set; }
        public AddressDto? ShippingAddress { get; set; }
        public List<OrderDetailsItemDto> Items { get; set; } = new();
        public List<OrderTimelineDto> Timeline { get; set; } = new();
    }

    public class OrderDetailsItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? VariantInfo { get; set; }
    }

    public class OrderTimelineDto
    {
        public DateTime Timestamp { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class AddressDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }

    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}