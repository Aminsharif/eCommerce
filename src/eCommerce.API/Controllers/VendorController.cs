using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Logging;

namespace eCommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorController : ControllerBase
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<VendorController> _logger;

        public VendorController(IVendorService vendorService, ILogger<VendorController> logger)
        {
            _vendorService = vendorService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var vendors = await _vendorService.GetAllVendors(page, pageSize);
                return Ok(vendors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving vendors");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Vendor>> GetVendor(int id)
        {
            try
            {
                var vendor = await _vendorService.GetVendorById(id);
                if (vendor == null)
                    return NotFound();

                return Ok(vendor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vendor {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Vendor>> CreateVendor([FromBody] Vendor vendor)
        {
            try
            {
                var createdVendor = await _vendorService.CreateVendor(vendor);
                return CreatedAtAction(nameof(GetVendor), new { id = createdVendor.Id }, createdVendor);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vendor");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Vendor>> UpdateVendor(int id, [FromBody] Vendor vendor)
        {
            try
            {
                if (id != vendor.Id)
                    return BadRequest();

                var updatedVendor = await _vendorService.UpdateVendor(vendor);
                return Ok(updatedVendor);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating vendor {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeactivateVendor(int id)
        {
            try
            {
                var result = await _vendorService.DeactivateVendor(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating vendor {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<Product>>> GetVendorProducts(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var products = await _vendorService.GetVendorProducts(id, page, pageSize);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving products for vendor {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetVendorOrders(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var orders = await _vendorService.GetVendorOrders(id, page, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for vendor {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/commissions")]
        public async Task<ActionResult<IEnumerable<Commission>>> GetVendorCommissions(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var commissions = await _vendorService.GetVendorCommissions(id, page, pageSize);
                return Ok(commissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving commissions for vendor {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("orders/{orderId}/calculate-commission")]
        public async Task<ActionResult<Commission>> CalculateCommission(int orderId)
        {
            try
            {
                var commission = await _vendorService.CalculateCommission(orderId);
                return Ok(commission);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error calculating commission for order {orderId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("commissions/{id}/mark-paid")]
        public async Task<ActionResult> MarkCommissionAsPaid(int id)
        {
            try
            {
                var result = await _vendorService.MarkCommissionAsPaid(id);
                if (!result)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking commission {id} as paid");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/analytics")]
        public async Task<ActionResult<VendorAnalytics>> GetVendorAnalytics(int id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var analytics = await _vendorService.GetVendorAnalytics(id, startDate, endDate);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving analytics for vendor {id}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 