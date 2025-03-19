using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IInventoryService
    {
        Task<Inventory> GetInventoryById(int id);
        Task<Inventory> GetInventoryByProductId(int productId);
        Task<IEnumerable<Inventory>> GetAllInventory(int page, int pageSize);
        Task<Inventory> CreateInventory(Inventory inventory);
        Task<Inventory> UpdateInventory(Inventory inventory);
        Task<bool> DeleteInventory(int id);
        Task<InventoryTransaction> AddStock(int inventoryId, int quantity, string reference, string notes);
        Task<InventoryTransaction> RemoveStock(int inventoryId, int quantity, string reference, string notes);
        Task<InventoryTransaction> AdjustStock(int inventoryId, int quantity, string reason);
        Task<InventoryTransaction> TransferStock(int fromInventoryId, int toInventoryId, int quantity, string reference, string notes);
        Task<IEnumerable<Inventory>> GetLowStockItems();
        Task<IEnumerable<InventoryTransaction>> GetInventoryTransactions(int inventoryId, DateTime startDate, DateTime endDate);
        Task<InventoryAnalytics> GetInventoryAnalytics(int inventoryId);
    }
} 