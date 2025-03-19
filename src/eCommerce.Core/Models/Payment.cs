using System;

namespace eCommerce.Core.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public required PaymentMethodType Method { get; set; }
        public required string Currency { get; set; }
        public required string TransactionId { get; set; }
        public required string PaymentProvider { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public required virtual Order Order { get; set; }
        public string? ErrorMessage { get; set; }
        public string? PaymentDetails { get; set; }
        public DateTime? CompletedAt { get; set; }
        public decimal? RefundAmount { get; set; }
    }
}