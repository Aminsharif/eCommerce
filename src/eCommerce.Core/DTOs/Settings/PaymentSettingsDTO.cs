using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Settings
{
    public class PaymentSettingsDto
    {
        public bool EnablePayPal { get; set; } = true;
        public string PayPalClientId { get; set; } = string.Empty;
        public string PayPalClientSecret { get; set; } = string.Empty;
        public bool EnableStripe { get; set; } = true;
        public string StripePublishableKey { get; set; } = string.Empty;
        public string StripeSecretKey { get; set; } = string.Empty;
        public bool EnableCashOnDelivery { get; set; } = true;
        public bool EnableBankTransfer { get; set; } = true;
        public string BankAccountDetails { get; set; } = string.Empty;
    }
}