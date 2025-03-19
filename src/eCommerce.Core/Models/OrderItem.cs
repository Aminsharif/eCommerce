using System;

namespace eCommerce.Core.Models
{
    public class OrderItem : BaseEntity
    {
        public OrderItem()
        {
            ProductName = string.Empty;
            ProductImage = string.Empty;
        }

        public int OrderId { get; set; }
        public required Order Order { get; set; }
        public int ProductId { get; set; }
        public required Product Product { get; set; }
        public string ProductName { get; set; }
        public string? ProductImage { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 