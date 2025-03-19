using System;

namespace eCommerce.Core.Models
{
    public class Address : BaseEntity
    {
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Country { get; set; }
        public required string PostalCode { get; set; }
        public bool IsDefault { get; set; }
        public required string AddressType { get; set; }
        public required int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
} 