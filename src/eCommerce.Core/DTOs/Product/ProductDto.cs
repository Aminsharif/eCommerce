using System;
using System.Collections.Generic;

namespace eCommerce.Core.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public List<string> Images { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? VendorId { get; set; }
        public string VendorName { get; set; }
        public int StockQuantity { get; set; }
        public decimal Rating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsActive { get; set; }
        public decimal Cost { get; set; }
        public string SKU { get; set; }
        public int ReorderPoint { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ProductListDto
    {
        public IEnumerable<ProductDto> Products { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class ProductFilterDto
    {
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? VendorId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
        public string SortBy { get; set; }
        public bool SortDescending { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
} 