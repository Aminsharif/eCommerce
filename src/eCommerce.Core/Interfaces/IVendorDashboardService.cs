using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.DTOs.Vendor;

namespace eCommerce.Core.Interfaces
{
    public interface IVendorDashboardService
    {
        Task<decimal> GetVendorTotalRevenueAsync(int vendorId);
        Task<int> GetVendorTotalProductsCountAsync(int vendorId);
        Task<int> GetVendorActiveProductsCountAsync(int vendorId);
        Task<int> GetVendorOutOfStockProductsCountAsync(int vendorId);
        Task<int> GetVendorTotalOrdersCountAsync(int vendorId);
        Task<int> GetVendorPendingOrdersCountAsync(int vendorId);
        Task<decimal> GetVendorAverageRatingAsync(int vendorId);
        Task<List<VendorDashboardDto.RecentOrder>> GetVendorRecentOrdersAsync(int vendorId, int count);
        Task<List<VendorDashboardDto.ProductPerformance>> GetVendorTopProductsAsync(int vendorId, int count);
        Task<List<VendorDashboardDto.SalesByMonth>> GetVendorSalesHistoryAsync(int vendorId, DateTime startDate, DateTime endDate);
        Task<VendorDashboardDto> GetVendorDashboardDataAsync(int vendorId);
    }
}