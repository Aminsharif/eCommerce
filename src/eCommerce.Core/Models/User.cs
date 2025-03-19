using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace eCommerce.Core.Models
{
    public class User : BaseEntity
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public new bool IsActive { get; set; }
        public new DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
        public virtual Cart? Cart { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? LastLogin { get; set; }
        public Vendor? Vendor { get; set; }
        public ICollection<Product>? WishList { get; set; }
        public UserRole Role { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        public User()
        {
            Orders = new List<Order>();
            Reviews = new List<Review>();
            Addresses = new List<Address>();
            WishList = new List<Product>();
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }
    }
} 