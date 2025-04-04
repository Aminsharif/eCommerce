using System;
using System.Threading.Tasks;
using eCommerce.Core.Models;using eCommerce.Core.DTOs.Admin;
using eCommerce.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace eCommerce.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IVendorService _vendorService;

        public AdminController(
            IOrderService orderService,
            IPaymentService paymentService,
            IProductService productService,
            IUserService userService,
            IVendorService vendorService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _productService = productService;
            _userService = userService;
            _vendorService = vendorService;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardDto>> GetDashboardData()
        {
            try
            {
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddMonths(-12);

                var dashboard = new AdminDashboardDto
                {
                    TotalRevenue = await _paymentService.GetTotalRevenueAsync(),
                    TotalOrders = await _orderService.GetTotalOrdersCountAsync(),
                    TotalProducts = await _productService.GetTotalProductsCountAsync(),
                    TotalCustomers = await _userService.GetTotalCustomersCountAsync(),
                    TotalVendors = await _vendorService.GetTotalVendorsCountAsync(),
                    PendingOrders = await _orderService.GetPendingOrdersCountAsync(),
                    LowStockProducts = await _productService.GetLowStockProductsCountAsync(),
                    RecentOrders = await _orderService.GetRecentOrdersAsync(10),
                    TopProducts = await _productService.GetTopSellingProductsAsync(10),
                    RevenueHistory = await _paymentService.GetRevenueHistoryAsync(startDate, endDate)
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching dashboard data.");
            }
        }

        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueStats>> GetRevenueStats()
        {
            var stats = await _paymentService.GetRevenueStats();
            return Ok(stats);
        }

        [HttpGet("analytics")]
        public async Task<ActionResult<Analytics>> GetAnalytics()
        {
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;
            var analytics = await _orderService.GetOrderAnalyticsAsync(startDate, endDate);
            return Ok(analytics);
        }
    }
}