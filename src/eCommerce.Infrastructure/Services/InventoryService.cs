using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using eCommerce.Core.Enums;
using Microsoft.Extensions.Logging;

namespace eCommerce.Infrastructure.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductService _productService;
        private readonly ILogger<InventoryService> _logger;
        private readonly ITransactionRepository _transactionRepository;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IProductService productService,
            ILogger<InventoryService> logger,
            ITransactionRepository transactionRepository)
        {
            _inventoryRepository = inventoryRepository;
            _productService = productService;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        public async Task<Inventory> GetInventoryById(int id)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(id);
            if (inventory == null)
                throw new KeyNotFoundException($"Inventory with ID {id} not found");
            return inventory;
        }

        public async Task<Inventory> GetInventoryByProductId(int productId)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
            if (inventory == null)
                throw new KeyNotFoundException($"Inventory not found for product {productId}");
            return inventory;
        }

        public async Task<IEnumerable<Inventory>> GetAllInventory(int page, int pageSize)
        {
            return await _inventoryRepository.GetAllInventoryAsync(page, pageSize);
        }

        public async Task<Inventory> CreateInventory(Inventory inventory)
        {
            try
            {
                var product = await _productService.GetProductById(inventory.ProductId);
                if (product == null)
                    throw new KeyNotFoundException($"Product with ID {inventory.ProductId} not found");

                inventory.CreatedAt = DateTime.UtcNow;
                inventory.LastRestocked = DateTime.UtcNow;
                return await _inventoryRepository.AddAsync(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory");
                throw;
            }
        }

        public async Task<Inventory> UpdateInventory(Inventory inventory)
        {
            try
            {
                var existingInventory = await _inventoryRepository.GetByIdAsync(inventory.Id);
                if (existingInventory == null)
                    throw new KeyNotFoundException($"Inventory with ID {inventory.Id} not found");

                existingInventory.Quantity = inventory.Quantity;
                existingInventory.ReorderPoint = inventory.ReorderPoint;
                existingInventory.ReorderQuantity = inventory.ReorderQuantity;
                existingInventory.Location = inventory.Location;
                existingInventory.UpdatedAt = DateTime.UtcNow;

                var updatedInventory = await _inventoryRepository.UpdateAsync(existingInventory);
                return updatedInventory;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating inventory {inventory.Id}");
                throw;
            }
        }

        public async Task<bool> DeleteInventory(int id)
        {
            try
            {
                var inventory = await _inventoryRepository.GetByIdAsync(id);
                if (inventory == null)
                {
                    return false;
                }

                await _inventoryRepository.DeleteAsync(inventory);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory {Id}", id);
                return false;
            }
        }

        public async Task<InventoryTransaction> AddStock(int inventoryId, int quantity, string reference, string notes)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found");
            }

            inventory.Quantity += quantity;
            inventory.LastModified = DateTime.UtcNow;
            await _inventoryRepository.UpdateAsync(inventory);

            var transaction = new InventoryTransaction
            {
                InventoryId = inventoryId,
                Type = TransactionType.Purchase,
                Quantity = quantity,
                UnitPrice = inventory.UnitCost,
                Reference = reference,
                Notes = notes,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task<InventoryTransaction> RemoveStock(int inventoryId, int quantity, string reference, string notes)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found");
            }

            if (inventory.Quantity < quantity)
            {
                throw new InvalidOperationException($"Insufficient stock. Available: {inventory.Quantity}, Requested: {quantity}");
            }

            inventory.Quantity -= quantity;
            inventory.LastModified = DateTime.UtcNow;
            await _inventoryRepository.UpdateAsync(inventory);

            var transaction = new InventoryTransaction
            {
                InventoryId = inventoryId,
                Type = TransactionType.Sale,
                Quantity = quantity,
                UnitPrice = inventory.UnitCost,
                Reference = reference,
                Notes = notes,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task<InventoryTransaction> AdjustStock(int inventoryId, int quantity, string reason)
        {
            var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found");
            }

            inventory.Quantity = quantity;
            inventory.LastModified = DateTime.UtcNow;
            await _inventoryRepository.UpdateAsync(inventory);

            var transaction = new InventoryTransaction
            {
                InventoryId = inventoryId,
                Type = TransactionType.Adjustment,
                Quantity = quantity,
                UnitPrice = inventory.UnitCost,
                Reference = "Stock Adjustment",
                Notes = reason,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task<InventoryTransaction> TransferStock(int fromInventoryId, int toInventoryId, int quantity, string reference, string notes)
        {
            var fromInventory = await _inventoryRepository.GetByIdAsync(fromInventoryId);
            var toInventory = await _inventoryRepository.GetByIdAsync(toInventoryId);

            if (fromInventory == null)
            {
                throw new KeyNotFoundException($"Source inventory with ID {fromInventoryId} not found");
            }

            if (toInventory == null)
            {
                throw new KeyNotFoundException($"Destination inventory with ID {toInventoryId} not found");
            }

            if (fromInventory.Quantity < quantity)
            {
                throw new InvalidOperationException($"Insufficient stock in source inventory. Available: {fromInventory.Quantity}, Requested: {quantity}");
            }

            fromInventory.Quantity -= quantity;
            toInventory.Quantity += quantity;
            fromInventory.LastModified = DateTime.UtcNow;
            toInventory.LastModified = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(fromInventory);
            await _inventoryRepository.UpdateAsync(toInventory);

            var transaction = new InventoryTransaction
            {
                InventoryId = fromInventoryId,
                Type = TransactionType.Transfer,
                Quantity = quantity,
                UnitPrice = fromInventory.UnitCost,
                Reference = reference,
                Notes = notes,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return await _transactionRepository.AddAsync(transaction);
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItems()
        {
            return await _inventoryRepository.GetLowStockItemsAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetInventoryTransactions(int inventoryId)
        {
            return await _transactionRepository.GetByInventoryIdAsync(inventoryId);
        }

        public async Task<InventoryAnalytics> GetInventoryAnalytics(int inventoryId)
        {
            var transactions = await _transactionRepository.GetByInventoryIdAsync(inventoryId);
            var inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found");
            }

            return new InventoryAnalytics
            {
                CurrentStock = inventory.Quantity,
                MinimumStockLevel = inventory.MinimumStockLevel,
                MaximumStockLevel = inventory.MaximumStockLevel,
                UnitCost = inventory.UnitCost,
                LastModified = inventory.LastModified,
                TransactionCount = transactions.Count()
            };
        }

        Task<IEnumerable<InventoryTransaction>> IInventoryService.GetInventoryTransactions(int inventoryId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
} 