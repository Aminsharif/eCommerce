using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eCommerce.Core.Models
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public int? ParentCategoryId { get; set; }
        
        [JsonIgnore]
        public Category? ParentCategory { get; set; }
        
        [JsonIgnore]
        public ICollection<Category> SubCategories { get; set; }
        
        [JsonIgnore]
        public ICollection<Product> Products { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ImageUrl { get; set; }

        public Category()
        {
            SubCategories = new List<Category>();
            Products = new List<Product>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            Description = string.Empty;
            Slug = string.Empty;
            ImageUrl = string.Empty;
            Name = string.Empty;
        }
    }
} 