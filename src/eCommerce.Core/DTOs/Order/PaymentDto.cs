using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Order
{
    public class PaymentDto
    {
        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = "USD";

        public string? TransactionId { get; set; }

        public string? PaymentStatus { get; set; }

        public string? ErrorMessage { get; set; }

        [Required]
        public PaymentDetailsDto PaymentDetails { get; set; } = new PaymentDetailsDto();
    }

    public class PaymentDetailsDto
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Expiry date must be in MM/YY format")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "CVV must be 3 digits")]
        public string CVV { get; set; } = string.Empty;

        [Required]
        public string CardHolderName { get; set; } = string.Empty;
    }
}