using System;
using System.Collections.Generic;

namespace eCommerce.Core.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public required string Title { get; set; }
        public required string Comment { get; set; }
        public int Rating { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public List<string> Images { get; set; } = new();
        public int LikesCount { get; set; }

        public User? User { get; set; }
        public Product? Product { get; set; }

        public Review()
        {
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            IsVerifiedPurchase = false;
        }
    }
} 