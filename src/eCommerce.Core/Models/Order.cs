using System;
using System.Collections.Generic;

namespace eCommerce.Core.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled,
        Refunded
    }

    public class Order : BaseEntity
    {
        public required int UserId { get; set; }
        public User? User { get; set; }
        public required string OrderNumber { get; set; }
        public required DateTime OrderDate { get; set; }
        public required OrderStatus Status { get; set; }
        public required decimal SubTotal { get; set; }
        public required decimal Tax { get; set; }
        public required decimal ShippingCost { get; set; }
        public required decimal DiscountAmount { get; set; }
        public required decimal Total { get; set; }
        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddress { get; set; }
        public int? BillingAddressId { get; set; }
        public Address? BillingAddress { get; set; }
        public required PaymentMethodType PaymentMethod { get; set; }
        public required PaymentStatus PaymentStatus { get; set; }
        public required string TrackingNumber { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public required string Notes { get; set; }
        public required ICollection<OrderItem> Items { get; set; }
        public required ICollection<Payment> Payments { get; set; }
        public required ICollection<Commission> Commissions { get; set; }
        public required virtual Vendor Vendor { get; set; }

        public Order()
        {
            Items = new List<OrderItem>();
            Payments = new List<Payment>();
            Commissions = new List<Commission>();
            CreatedAt = DateTime.UtcNow;
            OrderDate = DateTime.UtcNow;
            OrderNumber = Guid.NewGuid().ToString("N");
            Status = OrderStatus.Pending;
            PaymentStatus = PaymentStatus.Pending;
            PaymentMethod = PaymentMethodType.Other;
            TrackingNumber = string.Empty;
            Notes = string.Empty;
            SubTotal = 0;
            Tax = 0;
            ShippingCost = 0;
            DiscountAmount = 0;
            Total = 0;
        }
    }
}