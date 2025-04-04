using System;
using System.Threading.Tasks;
using eCommerce.Core.DTOs.Vendor;
using eCommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Authorize(Roles = "Vendor")]
    [ApiController]
    [Route("api/vendor/dashboard")]
    public class VendorDashboardController : ControllerBase
    {
        private readonly IVendorDashboardService _vendorDashboardService;

        public VendorDashboardController(IVendorDashboardService vendorDashboardService)
        {
            _vendorDashboardService = vendorDashboardService;
        }

        [HttpGet]
        public async Task<ActionResult<VendorDashboardDto>> GetDashboardData()
        {
            try
            {
                var vendorId = User.FindFirst("VendorId")?.Value;
                if (string.IsNullOrEmpty(vendorId) || !int.TryParse(vendorId, out int vendorIdInt))
                {
                    return BadRequest("Invalid vendor identification.");
                }

                var dashboard = await _vendorDashboardService.GetVendorDashboardDataAsync(vendorIdInt);

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching dashboard data.");
            }
        }
    }
}