using System;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Payment
{
    public class PaymentConfirmationDto
    {
        public string OrderId { get; set; } = string.Empty;

        public string TransactionId { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "USD";

        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public ReceiptInfo ReceiptDetails { get; set; } = new();

        public BillingInfo BillingDetails { get; set; } = new();
    }

    public class ReceiptInfo
    {
        public string ReceiptNumber { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }

        public decimal? Tax { get; set; }

        public decimal? ShippingCost { get; set; }

        public decimal? Discount { get; set; }

        public decimal TotalAmount { get; set; }

        public string? PromoCodeUsed { get; set; }
    }

    public class BillingInfo
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string PostalCode { get; set; } = string.Empty;
    }
}