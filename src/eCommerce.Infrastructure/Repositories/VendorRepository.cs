using eCommerce.Core.Enums;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class VendorRepository : GenericRepository<Vendor>, IVendorRepository
    {
        private readonly ApplicationDbContext _context;

        public VendorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Vendor> GetByUserIdAsync(int userId)
        {
            return await _context.Vendors
                .FirstOrDefaultAsync(v => v.UserId == userId);
        }

        public async Task<IEnumerable<Vendor>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Vendors
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Commission>> GetCommissionsAsync(int vendorId)
        {
            return await _context.Commissions
                .Where(c => c.VendorId == vendorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Commission> GetCommissionByIdAsync(int commissionId)
        {
            return await _context.Commissions
                .FirstOrDefaultAsync(c => c.Id == commissionId);
        }

        public async Task<Commission> AddCommissionAsync(Commission commission)
        {
            await _context.Commissions.AddAsync(commission);
            await _context.SaveChangesAsync();
            return commission;
        }

        public async Task<Commission> UpdateCommissionAsync(Commission commission)
        {
            _context.Entry(commission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return commission;
        }

        public async Task<IEnumerable<VendorReview>> GetVendorReviewsAsync(int vendorId)
        {
            return await _context.VendorReviews
                .Where(r => r.VendorId == vendorId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<VendorReview> GetVendorReviewByIdAsync(int reviewId)
        {
            return await _context.VendorReviews
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<IEnumerable<Product>> GetVendorProductsAsync(int vendorId)
        {
            return await _context.Products
                .Where(p => p.VendorId == vendorId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetVendorOrdersAsync(int vendorId)
        {
            return await _context.Orders
                .Where(o => o.Id == vendorId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<decimal> GetVendorRevenueAsync(int vendorId, DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Where(o => o.Id == vendorId && 
                           o.OrderDate >= startDate && 
                           o.OrderDate <= endDate)
                .SumAsync(o => o.Total);
        }

        public async Task<IEnumerable<Vendor>> GetActiveVendorsAsync()
        {
            return await _context.Vendors
                .Where(v => v.VendorStatus == VendorStatus.Active)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vendor>> GetInactiveVendorsAsync()
        {
            return await _context.Vendors
                .Where(v => v.VendorStatus == VendorStatus.Inactive)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vendor>> SearchVendorsAsync(string searchTerm)
        {
            return await _context.Vendors
                .Where(v => v.CompanyName.Contains(searchTerm) || 
                           v.Description.Contains(searchTerm))
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vendor>> GetVendorsByCategoryAsync(int categoryId)
        {
            return await _context.Vendors
                .Where(v => v.Products.Any(p => p.CategoryId == categoryId))
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vendor>> GetVendorsByRatingAsync(int minRating)
        {
            return await _context.Vendors
                .Where(v => v.Reviews.Average(r => r.Rating) >= minRating)
                .OrderByDescending(v => v.Reviews.Average(r => r.Rating))
                .ToListAsync();
        }

        public async Task<decimal> GetVendorAverageRatingAsync(int vendorId)
        {
            return (decimal)await _context.VendorReviews
                .Where(r => r.VendorId == vendorId)
                .AverageAsync(r => r.Rating);
        }

        public async Task<int> GetVendorReviewCountAsync(int vendorId)
        {
            return await _context.VendorReviews
                .Where(r => r.VendorId == vendorId)
                .CountAsync();
        }

        public async Task<IEnumerable<Vendor>> GetVendorsByCommissionRangeAsync(decimal minCommission, decimal maxCommission)
        {
            return await _context.Vendors
                .Where(v => v.Commissions.Any(c => 
                    c.CommissionAmount >= minCommission && c.CommissionAmount <= maxCommission))
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<decimal> GetVendorTotalCommissionAsync(int vendorId)
        {
            return await _context.Commissions
                .Where(c => c.VendorId == vendorId)
                .SumAsync(c => c.CommissionAmount);
        }

        public async Task<IEnumerable<Vendor>> GetVendorsByOrderCountAsync(int minOrders)
        {
            return await _context.Vendors
                .Where(v => v.Orders.Count >= minOrders)
                .OrderByDescending(v => v.Orders.Count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vendor>> GetVendorsByProductCountAsync(int minProducts)
        {
            return await _context.Vendors
                .Where(v => v.Products.Count >= minProducts)
                .OrderByDescending(v => v.Products.Count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vendor>> GetVendorsAsync(int page, int pageSize)
        {
            return await _context.Vendors
                .Include(v => v.User)
                .OrderByDescending(v => v.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetVendorProductsAsync(int vendorId, int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.VendorId == vendorId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetVendorOrdersAsync(int vendorId, int page, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.Items.Any(i => i.Product.VendorId == vendorId))
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<bool> UpdateCommissionStatusAsync(int commissionId, CommissionStatus status)
        {
            var commission = await _context.Commissions.FindAsync(commissionId);
            if (commission == null)
                return false;

            commission.Status = status;
            if (status == CommissionStatus.Paid)
                commission.PaidAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VendorReview> AddReviewAsync(VendorReview review)
        {
            await _context.VendorReviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<VendorReview> UpdateReviewAsync(VendorReview review)
        {
            _context.VendorReviews.Update(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteReviewAsync(int reviewId)
        {
            var review = await _context.VendorReviews.FindAsync(reviewId);
            if (review == null)
                return false;

            _context.VendorReviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<VendorReview> GetReviewByIdAsync(int reviewId)
        {
            return await _context.VendorReviews
                .Include(r => r.Vendor)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersByVendorAsync(int userId, int vendorId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .Where(o => o.UserId == userId && o.Items.Any(i => i.Product.VendorId == vendorId))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        Task<IEnumerable<Commission>> IVendorRepository.GetVendorCommissionsAsync(int vendorId, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<VendorReview> AddVendorReview(VendorReview review)
        {
            // Validate vendor exists
            var vendor = await _context.Vendors.FindAsync(review.VendorId);
            if (vendor == null)
            {
                throw new KeyNotFoundException($"Vendor with ID {review.VendorId} not found");
            }

            // Set timestamps
            review.CreatedAt = DateTime.UtcNow;
            review.UpdatedAt = DateTime.UtcNow;

            // Add the review
            await _context.VendorReviews.AddAsync(review);
            await _context.SaveChangesAsync();

            // Update vendor's average rating
            var averageRating = await _context.VendorReviews
                .Where(r => r.VendorId == review.VendorId)
                .AverageAsync(r => r.Rating);

            vendor.AverageRating = (decimal)averageRating;
            await _context.SaveChangesAsync();

            return review;
        }

        public async Task<bool> DeleteReview(int reviewId)
        {
            try
            {
                var review = await _context.VendorReviews.FindAsync(reviewId);
                if (review == null)
                {
                    return false;
                };
                _context.VendorReviews.Remove(review);
                await _context.SaveChangesAsync(); // Save changes to persist deletion
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteVendor(int reviewId)
        {
            try
            {
                var review = await _context.Vendors.FindAsync(reviewId);
                if (review == null)
                {
                    return false;
                };
                _context.Vendors.Remove(review);
                await _context.SaveChangesAsync(); // Save changes to persist deletion
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 