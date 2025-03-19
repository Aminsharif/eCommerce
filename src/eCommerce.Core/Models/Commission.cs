using System;

namespace eCommerce.Core.Models
{
    public class Commission
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public int OrderId { get; set; }
        public decimal OrderTotal { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal VendorAmount { get; set; }
        public CommissionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public required virtual Order Order { get; set; }
        public required virtual Vendor Vendor { get; set; }
    }

    public enum CommissionStatus
    {
        Pending,
        Calculated,
        Paid
    }
} 