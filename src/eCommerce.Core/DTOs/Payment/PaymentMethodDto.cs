using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Payment
{
    public class PaymentMethodDto
    {
        [Required]
        public string MethodType { get; set; } = string.Empty; // Credit Card, PayPal, etc.

        public bool IsDefault { get; set; }

        public string? DisplayName { get; set; }

        public string? Description { get; set; }

        public string? Icon { get; set; }

        public bool IsEnabled { get; set; } = true;

        public decimal? ProcessingFee { get; set; }

        public string[]? SupportedCurrencies { get; set; }

        // Credit Card specific properties
        public CreditCardInfo? CreditCardInfo { get; set; }

        // Digital Wallet specific properties
        public DigitalWalletInfo? DigitalWalletInfo { get; set; }
    }

    public class CreditCardInfo
    {
        [Required]
        [CreditCard]
        public string CardNumber { get; set; } = string.Empty;

        [Required]
        public string CardHolderName { get; set; } = string.Empty;

        [Required]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Expiry date must be in MM/YY format")]
        public string ExpiryDate { get; set; } = string.Empty;

        public string CardType { get; set; } = string.Empty; // Visa, MasterCard, etc.

        public string? LastFourDigits { get; set; }
    }

    public class DigitalWalletInfo
    {
        [Required]
        public string WalletType { get; set; } = string.Empty; // PayPal, Apple Pay, etc.

        [EmailAddress]
        public string? Email { get; set; }

        public string? AccountId { get; set; }
    }
}