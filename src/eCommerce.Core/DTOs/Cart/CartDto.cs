using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class AddToCartDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class UpdateCartItemDto
    {
        [Required]
        public int CartItemId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class RemoveFromCartDto
    {
        [Required]
        public int CartItemId { get; set; }
    }
} 