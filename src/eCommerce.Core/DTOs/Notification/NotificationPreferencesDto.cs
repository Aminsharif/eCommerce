using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Notification
{
    public class NotificationPreferencesDto
    {
        public bool EmailNotifications { get; set; } = true;

        public bool OrderStatusUpdates { get; set; } = true;

        public bool PromotionalEmails { get; set; } = true;

        public bool PriceAlerts { get; set; } = false;

        public bool StockNotifications { get; set; } = false;

        public bool NewsletterSubscription { get; set; } = true;

        public string? PreferredLanguage { get; set; }

        public string? TimeZone { get; set; }

        public NotificationFrequency EmailFrequency { get; set; } = NotificationFrequency.Immediately;
    }

    public enum NotificationFrequency
    {
        Immediately,
        Daily,
        Weekly,
        Monthly,
        Never
    }
}