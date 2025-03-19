using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetReviewsByProduct(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId && r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUser(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId && r.IsActive)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> AddReview(Review review)
        {
            try
            {
                review.CreatedAt = DateTime.UtcNow;
                review.IsActive = true;
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateReview(Review review)
        {
            try
            {
                review.UpdatedAt = DateTime.UtcNow;
                _context.Reviews.Update(review);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteReview(int reviewId)
        {
            try
            {
                var review = await _context.Reviews.FindAsync(reviewId);
                if (review == null)
                {
                    return false;
                }

                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync(); // Save changes to persist deletion
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByVendorIdAsync(int vendorId)
        {
            return await _context.Reviews
                .Where(r => r.Product.VendorId == vendorId)
                .ToListAsync();
        }

        public async Task<decimal> GetAverageRating(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (!reviews.Any())
            {
                return 0;
            }

            return (decimal)reviews.Average(r => r.Rating);
        }

        public async Task<IEnumerable<Review>> GetUserReviews(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetVendorReviewsAsync(int vendorId)
        {
            return await _context.Reviews
                .Where(r => r.Product.VendorId == vendorId)
                .ToListAsync();
        }
    }
} 