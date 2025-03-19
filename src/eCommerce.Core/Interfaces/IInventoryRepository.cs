using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IInventoryRepository
    {
        Task<Inventory> GetByIdAsync(int id);
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory> GetByProductIdAsync(int productId);
        Task<IEnumerable<Inventory>> GetAllInventoryAsync(int page, int pageSize);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync();
        Task<Inventory> AddAsync(Inventory inventory);
        Task<Inventory> UpdateAsync(Inventory inventory);
        Task DeleteAsync(Inventory inventory);
    }
} 