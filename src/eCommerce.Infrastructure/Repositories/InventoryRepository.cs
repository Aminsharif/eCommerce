using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using eCommerce.Core.Enums;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Inventory> GetByIdAsync(int id)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .ToListAsync();
        }

        public async Task<Inventory> GetByProductIdAsync(int productId)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        public async Task<IEnumerable<Inventory>> GetAllInventoryAsync(int page, int pageSize)
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inventory>> GetLowStockItemsAsync()
        {
            return await _context.Inventories
                .Include(i => i.Product)
                .Where(i => i.Quantity <= i.MinimumStockLevel)
                .ToListAsync();
        }

        public async Task<Inventory> AddAsync(Inventory inventory)
        {
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<Inventory> UpdateAsync(Inventory inventory)
        {
            _context.Entry(inventory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task DeleteAsync(Inventory inventory)
        {
            _context.Inventories.Remove(inventory);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsAsync(int inventoryId, DateTime startDate, DateTime endDate)
        {
            return await _context.InventoryTransactions
                .Where(t => t.InventoryId == inventoryId && 
                           t.CreatedAt >= startDate && 
                           t.CreatedAt <= endDate)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<InventoryTransaction> AddTransactionAsync(InventoryTransaction transaction)
        {
            await _context.InventoryTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<decimal> GetAverageStockValueAsync(int productId)
        {
            var inventory = await GetByProductIdAsync(productId);
            if (inventory == null)
                return 0;

            return inventory.Quantity * inventory.Product.Price;
        }

        public async Task<int> GetTotalTransactionsCountAsync(int productId, TransactionType type)
        {
            return await _context.InventoryTransactions
                .Where(t => t.Inventory.ProductId == productId && t.Type == type)
                .CountAsync();
        }
    }
} 