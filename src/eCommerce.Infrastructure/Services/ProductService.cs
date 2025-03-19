using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMemoryCache _cache;
        private const int CacheExpirationMinutes = 30;
        private readonly ILogger<ProductService> _logger;

        public ProductService(
            IProductRepository productRepository,
            IReviewRepository reviewRepository,
            IMemoryCache cache,
            ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _reviewRepository = reviewRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Product> GetProductById(int id)
        {
            string cacheKey = $"product_{id}";
            if (!_cache.TryGetValue(cacheKey, out Product product))
            {
                product = await _productRepository.GetByIdAsync(id);
                if (product != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(CacheExpirationMinutes));
                    _cache.Set(cacheKey, product, cacheOptions);
                }
            }
            if (product != null)
            {
                // Increment view count
                await _productRepository.UpdateAsync(product);
            }
            return product;
        }

        public async Task<IEnumerable<Product>> GetAllProducts(int page, int pageSize)
        {
            return await _productRepository.GetAllAsync(page, pageSize);
        }

        public async Task<IEnumerable<Product>> SearchProducts(string searchTerm, string category, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            return await _productRepository.SearchProductsAsync(searchTerm, category, minPrice, maxPrice, sortBy);
        }

        public async Task<Product> CreateProduct(Product product)
        {
            try
            {
                product.CreatedAt = DateTime.UtcNow;
                product.IsActive = true;
                var createdProduct = await _productRepository.AddAsync(product);
                _cache.Remove($"product_{createdProduct.Id}");
                return createdProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                throw;
            }
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            var existingProduct = await _productRepository.GetByIdAsync(product.Id);
            if (existingProduct == null)
            {
                throw new KeyNotFoundException($"Product with ID {product.Id} not found");
            }

            product.UpdatedAt = DateTime.UtcNow;
            return await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found");

            await _productRepository.DeleteAsync(product);
        }

        public async Task<bool> UpdateStock(int productId, int quantity)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return false;
                }

                product.StockQuantity = quantity;
                product.UpdatedAt = DateTime.UtcNow;
                await _productRepository.UpdateAsync(product);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stock for product {productId}");
                return false;
            }
        }

        public async Task<IEnumerable<Product>> GetRelatedProducts(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return Enumerable.Empty<Product>();

            return await _productRepository.GetByCategoryAsync(product.Category.Name);
        }

        public async Task<decimal> GetProductRating(int productId)
        {
            return await _productRepository.GetAverageRating(productId);
        }

        public async Task<IEnumerable<Review>> GetProductReviews(int productId)
        {
            return await _reviewRepository.GetReviewsByProduct(productId);
        }

        public async Task<bool> AddProductReview(Review review)
        {
            review.CreatedAt = DateTime.UtcNow;
            await _reviewRepository.AddAsync(review);

            // Update product rating
            var product = await _productRepository.GetByIdAsync(review.ProductId);
            if (product != null)
            {
                product.Rating = await _productRepository.GetAverageRating(review.ProductId);
                await _productRepository.UpdateAsync(product);
                _cache.Remove($"product_{review.ProductId}");
            }

            return true;
        }

        public async Task<IEnumerable<Product>> GetFeaturedProducts()
        {
            return await _productRepository.GetFeaturedProductsAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            return await _productRepository.GetProductsByCategory(categoryName);
        }

        public async Task<bool> IsProductInStock(int productId, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product != null && product.StockQuantity >= quantity;
        }

        public async Task<int> GetTotalProductsCount()
        {
            return await _productRepository.GetTotalCountAsync();
        }

        public async Task<IEnumerable<Product>> GetTopProducts(int count)
        {
            return await _productRepository.GetTopProductsAsync(count);
        }

        public async Task<IEnumerable<Product>> GetProducts(int page, int pageSize)
        {
            return await _productRepository.GetProductsAsync(page, pageSize);
        }

        public async Task<Analytics> GetProductAnalytics()
        {
            var products = await _productRepository.GetAllAsync();
            var analytics = new Analytics
            {
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow,
                TotalProducts = products.Count(),
                LowStockProductCount = products.Count(p => p.StockQuantity <= p.ReorderPoint),
                AverageProductPrice = products.Average(p => p.Price),
                InventoryValue = products.Sum(p => p.Price * p.StockQuantity),
                TopSellingProducts = (await _productRepository.GetTopSellingProductsAsync(10)).ToList(),
                TopViewedProducts = (await _productRepository.GetTopViewedProductsAsync(10)).ToList(),
                LowStockProducts = (await _productRepository.GetLowStockProductsAsync()).ToList(),
                CategoryInventory = (await _productRepository.GetCategoryInventoryAsync()).ToList(),
                DailySales = new List<DailySales>(),
                CategorySales = new List<CategorySales>(),
                PaymentMethodStats = new List<PaymentMethodStats>(),
                TotalRevenue = await _productRepository.GetTotalRevenueAsync(),
                TotalOrders = await _productRepository.GetTotalOrdersAsync(),
                TotalCustomers = await _productRepository.GetTotalCustomersAsync(),
                AverageOrderValue = await _productRepository.GetAverageOrderValueAsync(),
                ConversionRate = await _productRepository.GetConversionRateAsync(),
                TotalCost = await _productRepository.GetTotalCostAsync(),
                GrossProfit = await _productRepository.GetGrossProfitAsync(),
                NetProfit = await _productRepository.GetNetProfitAsync(),
                ProfitMargin = await _productRepository.GetProfitMarginAsync(),
                AverageProductCost = await _productRepository.GetAverageProductCostAsync(),
                InventoryTurnover = await _productRepository.GetInventoryTurnoverAsync(),
               DaysOfInventory = (int)Math.Round(await _productRepository.GetDaysOfInventoryAsync()),
            };

            return analytics;
        }

        public async Task<Analytics> GetAnalytics(DateTime startDate, DateTime endDate)
        {
            var analytics = new Analytics
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalRevenue = await _productRepository.GetTotalRevenueAsync(),
                TotalOrders = await _productRepository.GetTotalOrdersAsync(),
                TotalCustomers = await _productRepository.GetTotalCustomersAsync(),
                AverageOrderValue = await _productRepository.GetAverageOrderValueAsync(),
                ConversionRate = await _productRepository.GetConversionRateAsync(),
                TotalCost = await _productRepository.GetTotalCostAsync(),
                GrossProfit = await _productRepository.GetGrossProfitAsync(),
                NetProfit = await _productRepository.GetNetProfitAsync(),
                ProfitMargin = await _productRepository.GetProfitMarginAsync(),
                AverageProductCost = await _productRepository.GetAverageProductCostAsync(),
                InventoryTurnover = await _productRepository.GetInventoryTurnoverAsync(),
                DaysOfInventory = (int)Math.Round(await _productRepository.GetDaysOfInventoryAsync()),
                TotalProducts = (int)await _productRepository.GetTotalCountAsync(),
                LowStockProductCount = (await _productRepository.GetLowStockProductsAsync()).Count(),
                TopSellingProducts = (await _productRepository.GetTopSellingProductsAsync(10)).ToList(),
                TopViewedProducts = (await _productRepository.GetTopViewedProductsAsync(10)).ToList(),
                LowStockProducts = (await _productRepository.GetLowStockProductsAsync()).ToList(),
                CategoryInventory = (await _productRepository.GetCategoryInventoryAsync()).ToList(),
                DailySales = (await _productRepository.GetDailySalesAsync()).ToList(),
                CategorySales = (await _productRepository.GetCategorySalesAsync()).ToList(),
                PaymentMethodStats = (await _productRepository.GetPaymentMethodStatsAsync()).ToList()
            };

            return analytics;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            return await _productRepository.GetAllAsync(page, pageSize);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _productRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string categoryName)
        {
            return await _productRepository.GetByCategoryAsync(categoryName);
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _productRepository.GetFeaturedProductsAsync();
        }

        public async Task<decimal> GetAverageRating(int productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            if (!reviews.Any())
                return 0;

            return (decimal)reviews.Average(r => r.Rating);
        }

        public async Task<decimal> GetAveragePrice(string category)
        {
            var products = await _productRepository.GetByCategoryAsync(category);
            if (!products.Any())
            {
                return 0;
            }

            return products.Average(p => p.Price);
        }
    }
} 