using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IVendorRepository : IGenericRepository<Vendor>
    {
        Task<Vendor> GetByUserIdAsync(int userId);
        Task<IEnumerable<Vendor>> GetActiveVendorsAsync();
        Task<IEnumerable<Vendor>> GetVendorsAsync(int page, int pageSize);
        Task<IEnumerable<Product>> GetVendorProductsAsync(int vendorId, int page, int pageSize);
        Task<IEnumerable<Order>> GetVendorOrdersAsync(int vendorId, int page, int pageSize);
        Task<IEnumerable<Commission>> GetVendorCommissionsAsync(int vendorId, int page, int pageSize);
        Task<decimal> GetVendorRevenueAsync(int vendorId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetUserOrdersByVendorAsync(int userId, int vendorId);

        // Review-related methods
        Task<VendorReview> AddReviewAsync(VendorReview review);
        Task<VendorReview> UpdateReviewAsync(VendorReview review);
        Task<bool> DeleteReviewAsync(int reviewId);
        Task<VendorReview> GetReviewByIdAsync(int reviewId);
        Task<IEnumerable<VendorReview>> GetVendorReviewsAsync(int vendorId);

        Task<IEnumerable<Commission>> GetCommissionsAsync(int vendorId);

        Task<Commission> GetCommissionByIdAsync(int commissionId);
        Task<VendorReview> AddVendorReview(VendorReview review);
        Task<bool> DeleteReview(int reviewId);
        Task<bool> DeleteVendor(int id);
        Task<Commission> UpdateCommissionAsync(Commission commission);
    }
} 