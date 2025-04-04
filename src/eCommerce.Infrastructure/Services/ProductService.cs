using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eCommerce.Core.DTOs.Product;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using eCommerce.Core.DTOs.Vendor;
using eCommerce.Core.DTOs.Admin;

namespace eCommerce.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMemoryCache _cache;
        private const int CacheExpirationMinutes = 30;
        private readonly ILogger<ProductService> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IVendorRepository _vendorRepository;

        public async Task<int> GetTotalProductsCountAsync()
        {
            try
            {
                return await _productRepository.CountAsync(p => true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total products count");
                throw;
            }
        }

        public async Task<int> GetLowStockProductsCountAsync()
        {
            try
            {
                const int LowStockThreshold = 5;
                return await _productRepository.CountAsync(p => p.StockQuantity < LowStockThreshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting low stock products count");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count)
        {
            try
            {
                return await _productRepository.GetTopSellingProductsAsync(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting top {count} selling products");
                throw;
            }
        }

        public async Task<int> GetVendorProductsCountAsync(int vendorId)
        {
            try
            {
                return await _productRepository.CountAsync(p => p.VendorId == vendorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product count for vendor {vendorId}");
                throw;
            }
        }

        public async Task<int> GetVendorActiveProductsCountAsync(int vendorId)
        {
            try
            {
                return await _productRepository.CountAsync(p => p.VendorId == vendorId && p.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting active product count for vendor {vendorId}");
                throw;
            }
        }

        public async Task<int> GetVendorOutOfStockProductsCountAsync(int vendorId)
        {
            try
            {
                return await _productRepository.CountAsync(p => p.VendorId == vendorId && p.StockQuantity <= 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting out of stock product count for vendor {vendorId}");
                throw;
            }
        }

        public ProductService(
            IProductRepository productRepository,
            IReviewRepository reviewRepository,
            IMemoryCache cache,
            ILogger<ProductService> logger,
            ICategoryRepository categoryRepository,
            IVendorRepository vendorRepository)
        {
            _productRepository = productRepository;
            _reviewRepository = reviewRepository;
            _cache = cache;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _vendorRepository = vendorRepository;
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

        public async Task<ProductListDto> GetProductsAsync(ProductFilterDto filter, int page = 1, int pageSize = 10)
        {
            var query = _productRepository.GetQueryable()
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive);

            // Apply filters
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(filter.SearchTerm) || 
                                       p.Description.Contains(filter.SearchTerm));
            }

            if (filter.CategoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
            }

            if (filter.VendorId.HasValue)
            {
                query = query.Where(p => p.VendorId == filter.VendorId.Value);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            if (filter.InStock.HasValue && filter.InStock.Value)
            {
                query = query.Where(p => p.StockQuantity > 0);
            }

            // Apply sorting
            query = filter.SortBy?.ToLower() switch
            {
                "price" => filter.SortDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
                "name" => filter.SortDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                "rating" => filter.SortDescending ? query.OrderByDescending(p => p.Rating) : query.OrderBy(p => p.Rating),
                "created" => filter.SortDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var products = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new ProductListDto
            {
                Products = products.Select(MapToProductDto),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            };
        }

        public async Task<ProductDto> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToProductDto(product) : null;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId, int page = 1, int pageSize = 10)
        {
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId, page, pageSize);
            return products.Select(MapToProductDto);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByVendorAsync(int vendorId, int page = 1, int pageSize = 10)
        {
            var products = await _productRepository.GetProductsByVendorAsync(vendorId, page, pageSize);
            return products.Select(MapToProductDto);
        }

        public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync(int count = 10)
        {
            var products = await _productRepository.GetFeaturedProductsAsync(count);
            return products.Select(MapToProductDto);
        }

        public async Task<IEnumerable<ProductDto>> GetNewArrivalsAsync(int count = 10)
        {
            var products = await _productRepository.GetNewArrivalsAsync(count);
            return products.Select(MapToProductDto);
        }

        public async Task<IEnumerable<ProductDto>> GetBestSellersAsync(int count = 10)
        {
            var products = await _productRepository.GetBestSellersAsync(count);
            return products.Select(MapToProductDto);
        }

        public async Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId, int count = 4)
        {
            var products = await _productRepository.GetRelatedProductsAsync(productId, count);
            return products.Select(MapToProductDto);
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            var products = await _productRepository.SearchProductsAsync(searchTerm, page, pageSize);
            return products.Select(MapToProductDto);
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
        {
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                DiscountPrice = productDto.DiscountPrice,
                Cost = productDto.Cost,
                SKU = productDto.SKU,
                StockQuantity = productDto.StockQuantity,
                ReorderPoint = productDto.ReorderPoint,
                IsActive = productDto.IsActive,
                IsFeatured = productDto.IsFeatured,
                Brand = productDto.Brand,
                Manufacturer = productDto.Manufacturer,
                Weight = productDto.Weight,
                Dimensions = productDto.Dimensions,
                ShippingInfo = productDto.ShippingInfo,
                ReturnPolicy = productDto.ReturnPolicy,
                Warranty = productDto.Warranty,
                ViewCount = 0, // Default value
                Slug = productDto.Slug,
                ImagesJson = JsonSerializer.Serialize(productDto.Images ?? new List<string>()),
                SpecificationsJson = JsonSerializer.Serialize(productDto.Specifications ?? new Dictionary<string, string>()),
                CategoryId = productDto.CategoryId,
                VendorId = productDto.VendorId,
                Reviews = new List<Review>(), // Ensure required collections are initialized
                OrderItems = new List<OrderItem>(),
                CartItems = new List<CartItem>(),
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            return MapToProductDto(product);
        }

        public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            if (productDto.Name != null)
                product.Name = productDto.Name;
            if (productDto.Description != null)
                product.Description = productDto.Description;
            if (productDto.Price.HasValue)
                product.Price = productDto.Price.Value;
            if (productDto.DiscountPrice.HasValue)
                product.DiscountPrice = productDto.DiscountPrice.Value;
            if (productDto.Images != null)
                product.Images = productDto.Images;
            if (productDto.CategoryId.HasValue)
                product.CategoryId = productDto.CategoryId.Value;
            if (productDto.VendorId.HasValue)
                product.VendorId = productDto.VendorId.Value;
            if (productDto.StockQuantity.HasValue)
                product.StockQuantity = productDto.StockQuantity.Value;
            if (productDto.IsActive.HasValue)
                product.IsActive = productDto.IsActive.Value;

            product.UpdatedAt = DateTime.UtcNow;
            await _productRepository.UpdateAsync(product);
            return MapToProductDto(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            await _productRepository.DeleteAsync(product);
            return true;
        }

        public async Task<bool> UpdateProductStockAsync(int id, int quantity)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            product.StockQuantity = quantity;
            product.UpdatedAt = DateTime.UtcNow;
            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> UpdateProductStatusAsync(int id, bool isActive)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            product.IsActive = isActive;
            product.UpdatedAt = DateTime.UtcNow;
            await _productRepository.UpdateAsync(product);
            return true;
        }

        private ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                Images = product.Images,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                VendorId = product.VendorId,
                VendorName = product.Vendor?.Name,
                StockQuantity = product.StockQuantity,
                Rating = product.Rating,
                ReviewCount = product.Reviews?.Count ?? 0,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        Task<List<VendorDashboardDto.ProductPerformance>> IProductService.GetVendorTopProductsAsync(int vendorId, int count)
        {
            throw new NotImplementedException();
        }

        Task<List<AdminDashboardDto.TopProduct>> IProductService.GetTopSellingProductsAsync(int v)
        {
            throw new NotImplementedException();
        }
    }
}