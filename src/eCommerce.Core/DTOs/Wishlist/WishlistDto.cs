using System;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Wishlist
{
    public class WishlistDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime DateAdded { get; set; }
    }
}