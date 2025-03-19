using System;
using System.Collections.Generic;

namespace eCommerce.Core.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public string? AnonymousId { get; set; }
        public List<CartItem> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public string? CouponCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Cart()
        {
            Items = new List<CartItem>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public required Cart Cart { get; set; }
        public int ProductId { get; set; }
        public required Product Product { get; set; }
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public required string ProductImage { get; set; }

        public CartItem()
        {
            ProductName = string.Empty;
            ProductImage = string.Empty;
        }
    }
} 