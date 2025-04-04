namespace eCommerce.Core.DTOs.Settings
{
    public class GeneralSettingsDto
    {
        public string StoreName { get; set; } = string.Empty;
        public string StoreDescription { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Currency { get; set; } = "USD";
        public string DefaultLanguage { get; set; } = "en";
        public bool EnableNotifications { get; set; } = true;
        public bool EnableWishlist { get; set; } = true;
        public bool EnableReviews { get; set; } = true;
    }
}