using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;


namespace eCommerce.Core.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategory(string categoryName);
        Task<IReadOnlyList<Product>> GetProductsByVendor(int vendorId);
        Task<IEnumerable<Product>> GetByVendorIdAsync(int vendorId);
        Task<IReadOnlyList<Product>> SearchProducts(string searchTerm);
        Task<IReadOnlyList<Product>> GetFeaturedProducts();
        Task<IReadOnlyList<Product>> GetProductsOnSale();
        Task UpdateStockQuantity(int productId, int quantity);
        Task<bool> IsInStock(int productId);
        Task<decimal> GetAverageRating(int productId);
        Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count);
        Task<IEnumerable<Product>> GetTopViewedProductsAsync(int count);
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        Task<IEnumerable<CategoryInventory>> GetCategoryInventoryAsync();
        Task<int> GetTotalCountAsync();
        Task<IEnumerable<Product>> GetTopProductsAsync(int count);
        Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, string category, decimal? minPrice, decimal? maxPrice, string sortBy);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        Task<bool> IsProductInStock(int productId, int quantity);
        Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize);
        Task<decimal> GetAverageProductPrice();
        Task<decimal> GetTotalInventoryValue();
        Task<int> GetLowStockProductCount();
        Task<decimal> GetInventoryTurnoverRate();
        Task<decimal> GetDaysOfInventoryAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalOrdersAsync();
        Task<int> GetTotalCustomersAsync();
        Task<decimal> GetAverageOrderValueAsync();
        Task<decimal> GetConversionRateAsync();
        Task<decimal> GetTotalCostAsync();
        Task<decimal> GetGrossProfitAsync();
        Task<decimal> GetNetProfitAsync();
        Task<decimal> GetProfitMarginAsync();
        Task<decimal> GetAverageProductCostAsync();
        Task<decimal> GetInventoryTurnoverAsync();
        Task<IEnumerable<DailySales>> GetDailySalesAsync();
        Task<IEnumerable<CategorySales>> GetCategorySalesAsync();
        Task<IEnumerable<PaymentMethodStats>> GetPaymentMethodStatsAsync();
        Task<IEnumerable<Product>> SearchAsync(string query);
        Task<IEnumerable<Product>> GetByCategoryAsync(string category);
        Task<IEnumerable<Product>> GetFeaturedAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, int page, int pageSize);
        Task<IEnumerable<Product>> GetProductsByVendorAsync(int vendorId, int page, int pageSize);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count);
        Task<IEnumerable<Product>> GetNewArrivalsAsync(int count);
        Task<IEnumerable<Product>> GetBestSellersAsync(int count);
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int page, int pageSize);
        IQueryable<Product> GetQueryable();
    }
} 