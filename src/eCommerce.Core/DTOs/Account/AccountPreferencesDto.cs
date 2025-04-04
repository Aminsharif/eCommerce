using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Account
{
    public class AccountPreferencesDto
    {
        [Required]
        public string PreferredLanguage { get; set; } = "en";

        [Required]
        public string PreferredCurrency { get; set; } = "USD";

        public bool ReceiveNewsletters { get; set; } = true;

        public bool ReceiveOrderUpdates { get; set; } = true;

        public bool ReceivePromotionalEmails { get; set; } = true;

        public bool TwoFactorEnabled { get; set; } = false;

        [Required]
        public string TimeZone { get; set; } = "UTC";

        public bool DarkModeEnabled { get; set; } = false;
    }
}