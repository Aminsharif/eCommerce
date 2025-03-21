using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Core.Models
{
    public class Cart : BaseEntity
    {
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual ICollection<CartItem> Items { get; set; }
        public string? CouponCode { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int? ShippingAddressId { get; set; }
        public virtual Address? ShippingAddress { get; set; }
        public string? ShippingMethod { get; set; }
        public string? PaymentMethod { get; set; }
        public bool IsSavedForLater { get; set; }
        public string? SessionId { get; set; }

        public Cart()
        {
            Items = new HashSet<CartItem>();
            CreatedAt = DateTime.UtcNow;
            SubTotal = 0;
            Tax = 0;
            ShippingCost = 0;
            Discount = 0;
            Total = 0;
            IsSavedForLater = false;
        }
    }

    public class CartItem : BaseEntity
    {
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; } = null!;
        
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        
        public required string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public string? ProductImage { get; set; }
        public bool IsSavedForLater { get; set; }

        public CartItem()
        {
            CreatedAt = DateTime.UtcNow;
            Quantity = 1;
            IsSavedForLater = false;
        }
    }
} 