using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByProduct(int productId);
        Task<IEnumerable<Review>> GetByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetByVendorIdAsync(int vendorId);
        Task<decimal> GetAverageRating(int productId);
        Task<IEnumerable<Review>> GetUserReviews(int userId);
        Task<IEnumerable<Review>> GetVendorReviewsAsync(int vendorId);
        Task<bool> AddReview(Review review);
        Task<bool> UpdateReview(Review review);
        Task<bool> DeleteReview(int reviewId);
    }
} 