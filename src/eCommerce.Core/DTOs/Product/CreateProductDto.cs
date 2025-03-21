using System;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Product
{
    using eCommerce.Core.Models;
    using System.ComponentModel.DataAnnotations;

    public class CreateProductDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name must be at most 100 characters.")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Discount Price must be greater than 0.")]
        public decimal? DiscountPrice { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0.")]
        public decimal Cost { get; set; }

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public decimal Rating { get; set; } = 0;

        [Range(0, int.MaxValue, ErrorMessage = "Total reviews must be 0 or greater.")]
        public int TotalReviews { get; set; } = 0;

        [Required]
        public string SKU { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be 0 or greater.")]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder point must be 0 or greater.")]
        public int ReorderPoint { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public bool IsFeatured { get; set; } = false;

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public string Weight { get; set; }

        [Required]
        public string Dimensions { get; set; }

        [Required]
        public string ShippingInfo { get; set; }

        [Required]
        public string ReturnPolicy { get; set; }

        [Required]
        public string Warranty { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "View count must be 0 or greater.")]
        public int ViewCount { get; set; } = 0;

        [Required]
        public string Slug { get; set; }

        [Required]
        public List<string> Images { get; set; } = new List<string>();

        [Required]
        public Dictionary<string, string> Specifications { get; set; } = new Dictionary<string, string>();

        [Required]
        public int CategoryId { get; set; }

        public int? VendorId { get; set; } 

        [Required]
        public List<Review> Reviews { get; set; } = new List<Review>();

        [Required]
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        [Required]
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

}