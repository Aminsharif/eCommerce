using System;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Account
{
    public class AccountWishlistDto
    {
        public List<WishlistItem> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages { get; set; }

        public class WishlistItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public string ProductImage { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal? DiscountedPrice { get; set; }
            public bool IsInStock { get; set; }
            public DateTime DateAdded { get; set; }
            public string ProductUrl { get; set; } = string.Empty;
            public bool IsOnSale { get; set; }
            public string CategoryName { get; set; } = string.Empty;
        }
    }
}