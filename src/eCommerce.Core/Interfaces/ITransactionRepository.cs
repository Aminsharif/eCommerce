using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface ITransactionRepository
    {
        Task<InventoryTransaction> GetByIdAsync(int id);
        Task<IEnumerable<InventoryTransaction>> GetAllAsync();
        Task<IEnumerable<InventoryTransaction>> GetByInventoryIdAsync(int inventoryId);
        Task<InventoryTransaction> AddAsync(InventoryTransaction transaction);
        Task<InventoryTransaction> UpdateAsync(InventoryTransaction transaction);
        Task DeleteAsync(InventoryTransaction transaction);
    }
} 