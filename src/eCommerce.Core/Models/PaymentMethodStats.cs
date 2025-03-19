using System;

namespace eCommerce.Core.Models
{
    public class PaymentMethodStats
    {
        public int Id { get; set; }
        public required PaymentMethodType Method { get; set; }
        public int TotalTransactions { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AverageTransactionAmount { get; set; }
        public required decimal RefundRate { get; set; }
        public decimal ChargebackRate { get; set; }
        public decimal FailureRate { get; set; }
        public decimal ProcessingFees { get; set; }
        public decimal NetRevenue { get; set; }
        public decimal UsagePercentage { get; set; }
        public decimal ConversionRate { get; set; }
        public decimal CustomerPreference { get; set; }
        public decimal FraudRate { get; set; }
        public decimal AverageProcessingTime { get; set; }
        public decimal SuccessRate { get; set; }
    }
} 