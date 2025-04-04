using System.Threading.Tasks;
using eCommerce.Core.DTOs.Admin;
using eCommerce.Core.DTOs.Vendor;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<Payment> ProcessPaymentAsync(Order order, PaymentMethodType method, string paymentDetails);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
        Task<Payment> RefundPaymentAsync(int paymentId, decimal amount);
        Task<Payment> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status, string? transactionId = null);
        Task<bool> ValidatePaymentAsync(Payment payment);
        Task<RevenueStats> GetRevenueStats();

        Task<decimal> GetTotalRevenueAsync();
        // Vendor-specific methods
        Task<decimal> GetVendorRevenueAsync(int vendorId);
        Task<List<VendorDashboardDto.SalesByMonth>> GetVendorSalesHistoryAsync(int vendorId, DateTime startDate, DateTime endDate);
        Task<List<AdminDashboardDto.RevenueByMonth>> GetRevenueHistoryAsync(DateTime startDate, DateTime endDate);
    }
}