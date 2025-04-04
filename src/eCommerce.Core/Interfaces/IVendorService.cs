using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IVendorService
    {
        Task<Vendor> GetVendorById(int id);
        Task<Vendor> GetVendorByUserId(int userId);
        Task<IEnumerable<Vendor>> GetAllVendors(int page, int pageSize);
        Task<Vendor> CreateVendor(Vendor vendor);
        Task<Vendor> UpdateVendor(Vendor vendor);
        Task<bool> DeactivateVendor(int id);
        Task<bool> DeleteVendor(int id);
        Task<IEnumerable<Product>> GetVendorProducts(int vendorId, int page, int pageSize);
        Task<IEnumerable<Order>> GetVendorOrders(int vendorId, int page, int pageSize);
        Task<decimal> GetVendorRevenue(int vendorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Commission>> GetVendorCommissions(int vendorId, int page, int pageSize);
        Task<Commission> CalculateCommission(int orderId);
        Task<bool> MarkCommissionAsPaid(int commissionId);
        Task<VendorAnalytics> GetVendorAnalytics(int vendorId, DateTime startDate, DateTime endDate);

        // New review-related methods
        Task<VendorReview> AddVendorReview(VendorReview review);
        Task<VendorReview> UpdateVendorReview(VendorReview review);
        Task<bool> DeleteVendorReview(int reviewId);
        Task<int> GetTotalVendorsCountAsync();
        // Vendor-specific methods
        Task<decimal> GetVendorAverageRatingAsync(int vendorId);
    }
}