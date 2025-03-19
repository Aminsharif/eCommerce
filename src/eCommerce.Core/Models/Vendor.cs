using eCommerce.Core.Enums;
using System;
using System.Collections.Generic;

namespace eCommerce.Core.Models
{
    public class Vendor
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string CompanyName { get; set; }
        public required string Description { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string Address { get; set; }
        public required string LogoUrl { get; set; }
        public required string WebsiteUrl { get; set; }
        public decimal CommissionRate { get; set; }
        public VendorStatus VendorStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Commission> Commissions { get; set; }
        public ICollection<VendorReview> Reviews { get; set; }
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public Vendor()
        {
            Name = string.Empty;
            Description = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
            LogoUrl = string.Empty;
            WebsiteUrl = string.Empty;
            Products = new List<Product>();
            Orders = new List<Order>();
            Commissions = new List<Commission>();
            Reviews = new List<VendorReview>();
            CreatedAt = DateTime.UtcNow;
            VendorStatus = VendorStatus.Active;
            CommissionRate = 0;
            AverageRating = 0;
            TotalReviews = 0;
        }
    }
}