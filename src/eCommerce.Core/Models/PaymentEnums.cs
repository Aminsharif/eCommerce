namespace eCommerce.Core.Models
{
    public enum PaymentStatus
    {
        Pending,
        Processing,
        Completed,
        Failed,
        Refunded,
        Cancelled,
        Paid
    }

    public enum PaymentMethodType
    {
        CreditCard,
        DebitCard,
        BankTransfer,
        PayPal,
        Stripe,
        Cash,
        Other
    }
} 