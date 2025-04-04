using System;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Linq;
using eCommerce.Core.DTOs.Vendor;
using eCommerce.Core.DTOs.Admin;

namespace eCommerce.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IProductRepository _productRepository ;

        public async Task<decimal> GetTotalRevenueAsync()
        {
            try
            {
                return await _productRepository.GetTotalRevenueAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                throw;
            }
        }

 

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderService orderService,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<Payment> ProcessPaymentAsync(Order order, PaymentMethodType method, string paymentDetails)
        {
            try
            {
                var payment = new Payment
                {
                    Order = order,
                    OrderId = order.Id,
                    Amount = order.Total,
                    Method = method,
                    Status = PaymentStatus.Processing,
                    CreatedAt = DateTime.UtcNow,
                    PaymentDetails = paymentDetails,
                    Currency = "USD", // Default currency
                    PaymentProvider = method.ToString(),
                    TransactionId = Guid.NewGuid().ToString()
                };

                // Here you would integrate with actual payment providers (Stripe, PayPal, etc.)
                // For now, we'll simulate a successful payment
                payment.Status = PaymentStatus.Completed;
                payment.CompletedAt = DateTime.UtcNow;

                var createdPayment = await _paymentRepository.AddAsync(payment);
                
                // Update order status
                await _orderService.UpdatePaymentStatus(order.Id, PaymentStatus.Paid);

                return createdPayment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for order {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            return await _paymentRepository.GetByIdAsync(paymentId);
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _paymentRepository.GetByOrderIdAsync(orderId);
        }

        public async Task<Payment> RefundPaymentAsync(int paymentId, decimal amount)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found");

            if (payment.Status != PaymentStatus.Completed)
                throw new InvalidOperationException("Only completed payments can be refunded");

            if (amount > payment.Amount)
                throw new InvalidOperationException("Refund amount cannot exceed payment amount");

            payment.Status = PaymentStatus.Refunded;
            payment.ErrorMessage = $"Refunded {amount:C}";
            payment.RefundAmount = amount;
            
            // Here you would integrate with payment provider's refund API
            // For now, we'll just update the status

            return await _paymentRepository.UpdateAsync(payment);
        }

        public async Task<Payment> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status, string? transactionId = null)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

            payment.Status = status;
            if (transactionId != null)
                payment.TransactionId = transactionId;

            if (status == PaymentStatus.Completed)
                payment.CompletedAt = DateTime.UtcNow;

            return await _paymentRepository.UpdateAsync(payment);
        }

        public async Task<bool> ValidatePaymentAsync(Payment payment)
        {
            if (payment == null)
                return false;

            if (payment.Amount <= 0)
                return false;

            if (string.IsNullOrEmpty(payment.TransactionId))
                return false;

            // Here you would add additional validation logic
            // For example, verifying the payment with the payment provider
            // For now, we'll just do basic validation

            return true;
        }

        public async Task<RevenueStats> GetRevenueStats()
        {
            var stats = new RevenueStats();
            var now = DateTime.UtcNow;
            var startOfDay = now.Date;
            var startOfWeek = startOfDay.AddDays(-(int)startOfDay.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Get all completed payments
            var payments = await _paymentRepository.GetByStatusAsync(PaymentStatus.Completed);

            // Calculate total revenue
            stats.TotalRevenue = payments.Sum(p => p.Amount);

            // Calculate monthly revenue
            stats.MonthlyRevenue = payments
                .Where(p => p.CompletedAt >= startOfMonth)
                .Sum(p => p.Amount);

            // Calculate weekly revenue
            stats.WeeklyRevenue = payments
                .Where(p => p.CompletedAt >= startOfWeek)
                .Sum(p => p.Amount);

            // Calculate daily revenue
            stats.DailyRevenue = payments
                .Where(p => p.CompletedAt >= startOfDay)
                .Sum(p => p.Amount);

            return stats;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            try
            {
                payment.CreatedAt = DateTime.UtcNow;
                payment.Status = PaymentStatus.Pending;
                return await _paymentRepository.AddAsync(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                throw;
            }
        }

        public async Task<Payment> DeletePaymentAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {id} not found");
            }

            await _paymentRepository.DeleteAsync(payment);
            return payment;
        }

        public async Task<Payment> ProcessPayment(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {id} not found");
            }

            payment.Status = PaymentStatus.Completed;
            payment.CompletedAt = DateTime.UtcNow;
            await _paymentRepository.UpdateAsync(payment);
            return payment;
        }

        public async Task<Payment> UpdatePaymentStatus(int id, PaymentStatus status)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found");

            payment.Status = status;
            if (status == PaymentStatus.Completed)
                payment.CompletedAt = DateTime.UtcNow;

            var updatedPayment = await _paymentRepository.UpdateAsync(payment);
            return updatedPayment;
        }

        public async Task<Payment> RefundPayment(int id, decimal amount)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found");

            payment.Status = PaymentStatus.Refunded;
            payment.RefundAmount = amount;
            payment.CompletedAt = DateTime.UtcNow;
            var updatedPayment = await _paymentRepository.UpdateAsync(payment);
            return updatedPayment;
        }

        Task<decimal> IPaymentService.GetVendorRevenueAsync(int vendorId)
        {
            throw new NotImplementedException();
        }

        Task<List<VendorDashboardDto.SalesByMonth>> IPaymentService.GetVendorSalesHistoryAsync(int vendorId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        Task<List<AdminDashboardDto.RevenueByMonth>> IPaymentService.GetRevenueHistoryAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}