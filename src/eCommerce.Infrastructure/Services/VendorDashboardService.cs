using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Core.DTOs.Vendor;
using eCommerce.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Services
{
    public class VendorDashboardService : IVendorDashboardService
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;

        public VendorDashboardService(
            IOrderService orderService,
            IPaymentService paymentService,
            IProductService productService,
            IVendorService vendorService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _productService = productService;
            _vendorService = vendorService;
        }

        public async Task<decimal> GetVendorTotalRevenueAsync(int vendorId)
        {
            return await _paymentService.GetVendorRevenueAsync(vendorId);
        }

        public async Task<int> GetVendorTotalProductsCountAsync(int vendorId)
        {
            return await _productService.GetVendorProductsCountAsync(vendorId);
        }

        public async Task<int> GetVendorActiveProductsCountAsync(int vendorId)
        {
            return await _productService.GetVendorActiveProductsCountAsync(vendorId);
        }

        public async Task<int> GetVendorOutOfStockProductsCountAsync(int vendorId)
        {
            return await _productService.GetVendorOutOfStockProductsCountAsync(vendorId);
        }

        public async Task<int> GetVendorTotalOrdersCountAsync(int vendorId)
        {
            return await _orderService.GetVendorOrdersCountAsync(vendorId);
        }

        public async Task<int> GetVendorPendingOrdersCountAsync(int vendorId)
        {
            return await _orderService.GetVendorPendingOrdersCountAsync(vendorId);
        }

        public async Task<decimal> GetVendorAverageRatingAsync(int vendorId)
        {
            return await _vendorService.GetVendorAverageRatingAsync(vendorId);
        }

        public async Task<List<VendorDashboardDto.RecentOrder>> GetVendorRecentOrdersAsync(int vendorId, int count)
        {
            return await _orderService.GetVendorRecentOrdersAsync(vendorId, count);
        }

        public async Task<List<VendorDashboardDto.ProductPerformance>> GetVendorTopProductsAsync(int vendorId, int count)
        {
            return await _productService.GetVendorTopProductsAsync(vendorId, count);
        }

        public async Task<List<VendorDashboardDto.SalesByMonth>> GetVendorSalesHistoryAsync(int vendorId, DateTime startDate, DateTime endDate)
        {
            return await _paymentService.GetVendorSalesHistoryAsync(vendorId, startDate, endDate);
        }

        public async Task<VendorDashboardDto> GetVendorDashboardDataAsync(int vendorId)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-12);

            var dashboard = new VendorDashboardDto
            {
                TotalRevenue = await GetVendorTotalRevenueAsync(vendorId),
                TotalProducts = await GetVendorTotalProductsCountAsync(vendorId),
                ActiveProducts = await GetVendorActiveProductsCountAsync(vendorId),
                OutOfStockProducts = await GetVendorOutOfStockProductsCountAsync(vendorId),
                TotalOrders = await GetVendorTotalOrdersCountAsync(vendorId),
                PendingOrders = await GetVendorPendingOrdersCountAsync(vendorId),
                AverageRating = await GetVendorAverageRatingAsync(vendorId),
                RecentOrders = await GetVendorRecentOrdersAsync(vendorId, 10),
                TopPerformingProducts = await GetVendorTopProductsAsync(vendorId, 10),
                SalesHistory = await GetVendorSalesHistoryAsync(vendorId, startDate, endDate)
            };

            return dashboard;
        }
    }
}