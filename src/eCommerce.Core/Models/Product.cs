using System;
using System.Collections.Generic;
using System.Text.Json;

namespace eCommerce.Core.Models
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal DiscountPercentage 
        { 
            get => DiscountPrice.HasValue ? Math.Round((1 - DiscountPrice.Value / Price) * 100, 2) : 0;
        }
        public required decimal Cost { get; set; }
        public required decimal Rating { get; set; }
        public required int TotalReviews { get; set; }
        public required string SKU { get; set; }
        public required int StockQuantity { get; set; }
        public required int ReorderPoint { get; set; }
        public new required bool IsActive { get; set; }
        public required bool IsFeatured { get; set; }
        public required string Brand { get; set; }
        public required string Manufacturer { get; set; }
        public required string Weight { get; set; }
        public required string Dimensions { get; set; }
        public required string ShippingInfo { get; set; }
        public required string ReturnPolicy { get; set; }
        public required string Warranty { get; set; }
        public required int ViewCount { get; set; }
        public required string Slug { get; set; }
        public required string ImagesJson { get; set; } = string.Empty;
        public required string SpecificationsJson { get; set; } = string.Empty;
        public Dictionary<string, string> Specifications 
        { 
            get => string.IsNullOrEmpty(SpecificationsJson) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(SpecificationsJson) ?? new Dictionary<string, string>();
            set => SpecificationsJson = JsonSerializer.Serialize(value);
        }
        public List<string> Images
        {
            get => string.IsNullOrEmpty(ImagesJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(ImagesJson) ?? new List<string>();
            set => ImagesJson = JsonSerializer.Serialize(value);
        }

        public required int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? VendorId { get; set; }
        public Vendor? Vendor { get; set; }
        public required ICollection<Review> Reviews { get; set; }
        public required ICollection<OrderItem> OrderItems { get; set; }
        public required ICollection<CartItem> CartItems { get; set; }
        public Inventory? Inventory { get; set; }

        public Product()
        {
            Reviews = new List<Review>();
            OrderItems = new List<OrderItem>();
            CartItems = new List<CartItem>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            IsFeatured = false;
            Rating = 0;
            TotalReviews = 0;
            ViewCount = 0;
            Name = string.Empty;
            Description = string.Empty;
            SKU = string.Empty;
            Brand = string.Empty;
            Manufacturer = string.Empty;
            Weight = string.Empty;
            Dimensions = string.Empty;
            ShippingInfo = string.Empty;
            ReturnPolicy = string.Empty;
            Warranty = string.Empty;
            Slug = string.Empty;
            Price = 0;
            Cost = 0;
            StockQuantity = 0;
            ReorderPoint = 10;
        }
    }
}