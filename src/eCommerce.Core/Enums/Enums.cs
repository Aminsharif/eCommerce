namespace eCommerce.Core.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4,
        Refunded = 5,
        Cancelled = 6
    }

    public enum OrderStatus
    {
        Pending = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Refunded = 6
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        PayPal,
        BankTransfer,
        CashOnDelivery,
        DigitalWallet,
        Cryptocurrency
    }

    public enum TransactionType
    {
        Purchase,
        Sale,
        Return,
        Adjustment,
        Transfer,
        Damage,
        Loss
    }
    public enum VendorStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3,
        Pending = 4,
        Rejected = 5,
        Deleted = 6
    }
} 