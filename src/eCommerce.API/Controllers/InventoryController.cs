using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(
            IInventoryService inventoryService,
            ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetAllInventory([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var inventory = await _inventoryService.GetAllInventory(page, pageSize);
                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all inventory");
                return StatusCode(500, "An error occurred while retrieving inventory");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Inventory>> GetInventoryById(int id)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryById(id);
                if (inventory == null)
                    return NotFound($"Inventory with ID {id} not found");

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting inventory {id}");
                return StatusCode(500, "An error occurred while retrieving inventory");
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<Inventory>> GetInventoryByProductId(int productId)
        {
            try
            {
                var inventory = await _inventoryService.GetInventoryByProductId(productId);
                if (inventory == null)
                    return NotFound($"Inventory for product {productId} not found");

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting inventory for product {productId}");
                return StatusCode(500, "An error occurred while retrieving inventory");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Inventory>> CreateInventory([FromBody] Inventory inventory)
        {
            try
            {
                var createdInventory = await _inventoryService.CreateInventory(inventory);
                return CreatedAtAction(nameof(GetInventoryById), new { id = createdInventory.Id }, createdInventory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory");
                return StatusCode(500, "An error occurred while creating inventory");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Inventory>> UpdateInventory(int id, [FromBody] Inventory inventory)
        {
            try
            {
                if (id != inventory.Id)
                    return BadRequest("ID mismatch");

                var updatedInventory = await _inventoryService.UpdateInventory(inventory);
                return Ok(updatedInventory);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating inventory {id}");
                return StatusCode(500, "An error occurred while updating inventory");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteInventory(int id)
        {
            try
            {
                var result = await _inventoryService.DeleteInventory(id);
                if (!result)
                    return NotFound($"Inventory with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting inventory {id}");
                return StatusCode(500, "An error occurred while deleting inventory");
            }
        }

        [HttpPost("add-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryTransaction>> AddStock([FromBody] AddStockRequest request)
        {
            try
            {
                var transaction = await _inventoryService.AddStock(
                    request.ProductId,
                    request.Quantity,
                    request.Reference,
                    request.Notes);

                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding stock for product {request.ProductId}");
                return StatusCode(500, "An error occurred while adding stock");
            }
        }

        [HttpPost("remove-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryTransaction>> RemoveStock([FromBody] RemoveStockRequest request)
        {
            try
            {
                var transaction = await _inventoryService.RemoveStock(
                    request.ProductId,
                    request.Quantity,
                    request.Reference,
                    request.Notes);

                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing stock for product {request.ProductId}");
                return StatusCode(500, "An error occurred while removing stock");
            }
        }

        [HttpPost("adjust-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryTransaction>> AdjustStock([FromBody] AdjustStockRequest request)
        {
            try
            {
                var transaction = await _inventoryService.AdjustStock(
                    request.ProductId,
                    request.Quantity,
                    request.Reason);

                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adjusting stock for product {request.ProductId}");
                return StatusCode(500, "An error occurred while adjusting stock");
            }
        }

        [HttpPost("transfer-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryTransaction>> TransferStock([FromBody] TransferStockRequest request)
        {
            try
            {
                var transaction = await _inventoryService.TransferStock(
                    request.FromInventoryId,
                    request.ToInventoryId,
                    request.Quantity,
                    request.Reference,
                    request.Notes);

                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error transferring stock from {request.FromInventoryId} to {request.ToInventoryId}");
                return StatusCode(500, "An error occurred while transferring stock");
            }
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Inventory>>> GetLowStockItems()
        {
            try
            {
                var items = await _inventoryService.GetLowStockItems();
                return Ok(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock items");
                return StatusCode(500, "An error occurred while retrieving low stock items");
            }
        }

        [HttpGet("transactions/{productId}")]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetInventoryTransactions(
            int productId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var transactions = await _inventoryService.GetInventoryTransactions(productId, startDate, endDate);
                return Ok(transactions);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting transactions for product {productId}");
                return StatusCode(500, "An error occurred while retrieving transactions");
            }
        }

        [HttpGet("analytics/{productId}")]
        public async Task<ActionResult<InventoryAnalytics>> GetInventoryAnalytics(int productId)
        {
            try
            {
                var analytics = await _inventoryService.GetInventoryAnalytics(productId);
                return Ok(analytics);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting analytics for product {productId}");
                return StatusCode(500, "An error occurred while retrieving analytics");
            }
        }
    }

    public class AddStockRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }
    }

    public class RemoveStockRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reference { get; set; }
        public string Notes { get; set; }
    }

    public class AdjustStockRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
    }

    public class TransferStockRequest
    {
        public int FromInventoryId { get; set; }
        public int ToInventoryId { get; set; }
        public int Quantity { get; set; }
        public required string Notes { get; set; }
        public required string Reference { get; set; }
    }
} 