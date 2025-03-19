using System;

namespace eCommerce.Core.Models
{
    public class VendorReview
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation properties
        public Vendor Vendor { get; set; }
        public User User { get; set; }

        public VendorReview()
        {
            Comment = string.Empty;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Rating = 0;
        }
    }
} 