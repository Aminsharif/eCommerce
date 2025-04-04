using System;
using System.ComponentModel.DataAnnotations;

namespace eCommerce.Core.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public bool IsSuccess { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty; // Processing, Completed, Failed, Refunded

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "USD";

        public DateTime TransactionDate { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string? AuthorizationCode { get; set; }

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

        public string? ErrorDetails { get; set; }

        public PaymentGatewayInfo? GatewayInfo { get; set; }

        public RefundInfo? RefundInfo { get; set; }
    }

    public class PaymentGatewayInfo
    {
        public string GatewayName { get; set; } = string.Empty;

        public string? GatewayTransactionId { get; set; }

        public string? GatewayResponse { get; set; }

        public decimal? ProcessingFee { get; set; }
    }

    public class RefundInfo
    {
        public string? RefundId { get; set; }

        public decimal? RefundAmount { get; set; }

        public DateTime? RefundDate { get; set; }

        public string? RefundReason { get; set; }

        public string? RefundStatus { get; set; }
    }
}