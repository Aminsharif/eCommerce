using System;
using System.Threading.Tasks;
using eCommerce.Core.Models;
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

        public AdminController(
            IOrderService orderService,
            IPaymentService paymentService,
            IProductService productService)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _productService = productService;
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