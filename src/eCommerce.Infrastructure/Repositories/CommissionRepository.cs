using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Core.Enums;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class CommissionRepository : GenericRepository<Commission>, ICommissionRepository
    {
        private new readonly ApplicationDbContext _context;

        public CommissionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Commission> AddCommissionAsync(Commission commission)
        {
            // Validate commission data
            if (commission == null)
            {
                throw new ArgumentNullException(nameof(commission));
            }

            // Validate required fields
            if (commission.OrderId <= 0 || commission.VendorId <= 0)
            {
                throw new ArgumentException("OrderId and VendorId are required");
            }

            // Set timestamps if not set
            if (commission.CreatedAt == default)
            {
                commission.CreatedAt = DateTime.UtcNow;
            }

            // Add the commission
            await _context.Commissions.AddAsync(commission);
            await _context.SaveChangesAsync();

            // Reload the commission with related entities
            return await _context.Commissions
                .Include(c => c.Order)
                .Include(c => c.Vendor)
                .FirstOrDefaultAsync(c => c.Id == commission.Id);
        }

        public async Task<IEnumerable<Commission>> GetByVendorIdAsync(int vendorId)
        {
            return await _context.Commissions
                .Where(c => c.VendorId == vendorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Commission>> GetByStatusAsync(CommissionStatus status)
        {
            return await _context.Commissions
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalCommissionsByVendorAsync(int vendorId)
        {
            return await _context.Commissions
                .Where(c => c.VendorId == vendorId)
                .SumAsync(c => c.CommissionAmount);
        }

        public async Task<decimal> GetTotalCommissionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Commissions
                .Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate)
                .SumAsync(c => c.CommissionAmount);
        }

        public async Task<IEnumerable<Commission>> GetPendingCommissionsAsync()
        {
            return await _context.Commissions
                .Where(c => c.Status == CommissionStatus.Pending)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Commission>> GetPaidCommissionsAsync()
        {
            return await _context.Commissions
                .Where(c => c.Status == CommissionStatus.Paid)
                .OrderByDescending(c => c.PaidAt)
                .ToListAsync();
        }

        public async Task<bool> MarkAsPaidAsync(int commissionId)
        {
            var commission = await _context.Commissions.FindAsync(commissionId);
            if (commission == null)
            {
                return false;
            }

            commission.Status = CommissionStatus.Paid;
            commission.PaidAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Commission>> GetByOrderIdAsync(int orderId)
        {
            return await _context.Commissions
                .Where(c => c.OrderId == orderId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetVendorCommissionByDateRangeAsync(int vendorId, DateTime startDate, DateTime endDate)
        {
            return await _context.Commissions
                .Where(c => c.VendorId == vendorId && 
                           c.CreatedAt >= startDate && 
                           c.CreatedAt <= endDate)
                .SumAsync(c => c.CommissionAmount);
        }

        public async Task<IEnumerable<Commission>> GetCommissionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Commissions
                .Where(c => c.CreatedAt >= startDate && c.CreatedAt <= endDate)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Commission?> GetByIdAsync(int id)
        {
            return await _context.Commissions
                .Include(c => c.Vendor)
                .Include(c => c.Order)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
} 