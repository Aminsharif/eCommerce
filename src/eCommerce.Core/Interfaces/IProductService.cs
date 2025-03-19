using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetAllProducts(int page, int pageSize);
        Task<IEnumerable<Product>> SearchProducts(string searchTerm, string category, decimal? minPrice, decimal? maxPrice, string sortBy);
        Task<Product> CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task DeleteProduct(int id);
        Task<bool> UpdateStock(int productId, int quantity);
        Task<IEnumerable<Product>> GetRelatedProducts(int productId);
        Task<decimal> GetProductRating(int productId);
        Task<IEnumerable<Review>> GetProductReviews(int productId);
        Task<bool> AddProductReview(Review review);
        Task<IEnumerable<Product>> GetFeaturedProducts();
        Task<IEnumerable<Product>> GetProductsByCategory(string category);
        Task<bool> IsProductInStock(int productId, int quantity);
        Task<int> GetTotalProductsCount();
        Task<IEnumerable<Product>> GetTopProducts(int count);
        Task<IEnumerable<Product>> GetProducts(int page, int pageSize);
        Task<Analytics> GetProductAnalytics();
    }
} 