using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using eCommerce.Infrastructure.Data;

namespace eCommerce.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryTransaction> GetByIdAsync(int id)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Inventory)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<InventoryTransaction>> GetAllAsync()
        {
            return await _context.InventoryTransactions
                .Include(t => t.Inventory)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetByInventoryIdAsync(int inventoryId)
        {
            return await _context.InventoryTransactions
                .Include(t => t.Inventory)
                .Where(t => t.InventoryId == inventoryId)
                .ToListAsync();
        }

        public async Task<InventoryTransaction> AddAsync(InventoryTransaction transaction)
        {
            await _context.InventoryTransactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<InventoryTransaction> UpdateAsync(InventoryTransaction transaction)
        {
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteAsync(InventoryTransaction transaction)
        {
            _context.InventoryTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
} 