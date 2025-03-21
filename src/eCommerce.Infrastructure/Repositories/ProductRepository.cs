using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using eCommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private new readonly ApplicationDbContext _context;
        private readonly IReviewRepository _reviewRepository;

        public ProductRepository(ApplicationDbContext context, IReviewRepository reviewRepository) : base(context)
        {
            _context = context;
            _reviewRepository = reviewRepository;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategory(string categoryName)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.Name == categoryName)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsByVendor(int vendorId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.VendorId == vendorId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> SearchProducts(string searchTerm)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)) && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetFeaturedProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Rating)
                .Take(10)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Product>> GetProductsOnSale()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && p.DiscountPercentage > 0)
                .OrderByDescending(p => p.DiscountPercentage)
                .ToListAsync();
        }

        public async Task UpdateStockQuantity(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                product.StockQuantity = quantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsInStock(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && product.StockQuantity > 0;
        }

        public async Task<decimal> GetAverageRating(int productId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => r.Rating)
                .ToListAsync();

            return reviews.Any() ? (decimal)reviews.Average() : 0m;
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product != null)
            {
                // Deserialize Images
                if (!string.IsNullOrEmpty(product.ImagesJson))
                {
                    try
                    {
                        product.Images = JsonSerializer.Deserialize<List<string>>(product.ImagesJson);
                    }
                    catch
                    {
                        product.Images = new List<string>();
                    }
                }

                // Deserialize Specifications
                if (!string.IsNullOrEmpty(product.SpecificationsJson))
                {
                    try
                    {
                        product.Specifications = JsonSerializer.Deserialize<Dictionary<string, string>>(product.SpecificationsJson);
                    }
                    catch
                    {
                        product.Specifications = new Dictionary<string, string>();
                    }
                }
            }

            return product;
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.OrderItems)
                .OrderByDescending(p => p.OrderItems.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetTopViewedProductsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.ViewCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.StockQuantity <= p.ReorderPoint)
                .ToListAsync();
        }

        public async Task<IEnumerable<CategoryInventory>> GetCategoryInventoryAsync()
        {
            var categories = await _context.Products
                .Include(p => p.Category)
                .GroupBy(p => p.Category)
                .Select(g => new CategoryInventory
                {
                    Category = g.Key.Name,
                    TotalProducts = g.Count(),
                    LowStockProducts = g.Count(p => p.StockQuantity <= p.ReorderPoint),
                    TotalValue = g.Sum(p => p.Price * p.StockQuantity),
                    AveragePrice = g.Average(p => p.Price),
                    AverageCost = g.Average(p => p.Cost),
                    AverageOrderValue = g.SelectMany(p => p.OrderItems).Any() 
                        ? g.SelectMany(p => p.OrderItems).Average(i => i.UnitPrice * i.Quantity)
                        : 0,
                    ConversionRate = g.SelectMany(p => p.OrderItems).Any()
                        ? (decimal)g.SelectMany(p => p.OrderItems).Select(i => i.Order.UserId).Distinct().Count() / _context.Users.Count()
                        : 0,
                    ProfitMargin = g.Average(p => p.Price) > 0 
                        ? (g.Average(p => p.Price) - g.Average(p => p.Cost)) / g.Average(p => p.Price)
                        : 0,
                    InventoryTurnover = g.Sum(p => p.StockQuantity) > 0
                        ? g.SelectMany(p => p.OrderItems).Sum(i => i.Quantity) / (decimal)g.Sum(p => p.StockQuantity)
                        : 0,
                    DaysOfInventory = g.SelectMany(p => p.OrderItems).Any()
                        ? (int)(g.Sum(p => p.StockQuantity) / (g.SelectMany(p => p.OrderItems).Sum(i => i.Quantity) / 365.0))
                        : 0
                })
                .ToListAsync();

            return categories;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<IEnumerable<Product>> GetTopProductsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Reviews.Average(r => r.Rating))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, string category, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category.Name == category);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            query = sortBy?.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name" => query.OrderBy(p => p.Name),
                "rating" => query.OrderByDescending(p => p.Reviews.Average(r => r.Rating)),
                _ => query.OrderBy(p => p.Name)
            };

            return await query.ToListAsync();
        }

        public override async Task<Product> AddAsync(Product product)
        {
            // Serialize Images
            if (product.Images != null)
            {
                product.ImagesJson = JsonSerializer.Serialize(product.Images);
            }

            // Serialize Specifications
            if (product.Specifications != null)
            {
                product.SpecificationsJson = JsonSerializer.Serialize(product.Specifications);
            }

            return await base.AddAsync(product);
        }

        public override async Task<Product> UpdateAsync(Product product)
        {
            // Serialize Images
            if (product.Images != null)
            {
                product.ImagesJson = JsonSerializer.Serialize(product.Images);
            }

            // Serialize Specifications
            if (product.Specifications != null)
            {
                product.SpecificationsJson = JsonSerializer.Serialize(product.Specifications);
            }

            return await base.UpdateAsync(product);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.Name == category && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.IsActive)
                .OrderByDescending(p => p.Rating)
                .ToListAsync();
        }

        public async Task<bool> IsProductInStock(int productId, int quantity)
        {
            var product = await _context.Products.FindAsync(productId);
            return product != null && product.StockQuantity >= quantity;
        }

        public async Task<IEnumerable<Product>> GetAllAsync(int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<decimal> GetAverageProductPrice()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .AverageAsync(p => p.Price);
        }

        public async Task<decimal> GetTotalInventoryValue()
        {
            return await _context.Products
                .Where(p => p.IsActive)
                .SumAsync(p => p.Price * p.StockQuantity);
        }

        public async Task<int> GetLowStockProductCount()
        {
            return await _context.Products
                .CountAsync(p => p.StockQuantity <= p.ReorderPoint);
        }

        public async Task<decimal> GetInventoryTurnoverRate()
        {
            var totalInventoryValue = await GetTotalInventoryValue();
            var soldItems = await _context.OrderItems
                .Where(oi => oi.Order.CreatedAt >= DateTime.UtcNow.AddDays(-365))
                .SumAsync(oi => oi.Quantity * oi.UnitPrice);

            return totalInventoryValue > 0 ? soldItems / totalInventoryValue : 0;
        }

        public async Task<decimal> GetDaysOfInventoryAsync()
        {
            var inventoryTurnover = await GetInventoryTurnoverAsync();
            return inventoryTurnover > 0 ? 365m / inventoryTurnover : 0;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.Total);
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            return await _context.Users
                .Where(u => u.Orders.Any())
                .CountAsync();
        }

        public async Task<decimal> GetAverageOrderValueAsync()
        {
            return await _context.Orders
                .Where(o => o.PaymentStatus == PaymentStatus.Paid)
                .AverageAsync(o => o.Total);
        }

        public async Task<decimal> GetConversionRateAsync()
        {
            var totalVisitors = await _context.Users.CountAsync();
            var customersWithOrders = await GetTotalCustomersAsync();
            return totalVisitors > 0 ? (decimal)customersWithOrders / totalVisitors * 100 : 0;
        }

        public async Task<decimal> GetTotalCostAsync()
        {
            return await _context.Products
                .SumAsync(p => p.Cost * p.StockQuantity);
        }

        public async Task<decimal> GetGrossProfitAsync()
        {
            var revenue = await GetTotalRevenueAsync();
            var cost = await GetTotalCostAsync();
            return revenue - cost;
        }

        public async Task<decimal> GetNetProfitAsync()
        {
            var grossProfit = await GetGrossProfitAsync();
            // Assume operating expenses are 30% of revenue
            var revenue = await GetTotalRevenueAsync();
            var operatingExpenses = revenue * 0.3m;
            return grossProfit - operatingExpenses;
        }

        public async Task<decimal> GetProfitMarginAsync()
        {
            var revenue = await GetTotalRevenueAsync();
            var netProfit = await GetNetProfitAsync();
            return revenue > 0 ? (netProfit / revenue) * 100 : 0;
        }

        public async Task<decimal> GetAverageProductCostAsync()
        {
            return await _context.Products
                .AverageAsync(p => p.Cost);
        }

        public async Task<decimal> GetInventoryTurnoverAsync()
        {
            var averageInventoryValue = await _context.Products
                .AverageAsync(p => p.Cost * p.StockQuantity);
            var costOfGoodsSold = await GetTotalCostAsync();
            return averageInventoryValue > 0 ? costOfGoodsSold / averageInventoryValue : 0;
        }

        public async Task<IEnumerable<DailySales>> GetDailySalesAsync()
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);

            var dailySales = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.PaymentStatus == PaymentStatus.Paid)
                .GroupBy(o => new { o.OrderDate.Date, o.Items.First().Product.Category.Name })
                .Select(g => new DailySales
                {
                    Category = g.Key.Name,
                    Date = g.Key.Date,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.Total),
                    AverageOrderValue = g.Average(o => o.Total),
                    ItemsSold = g.Sum(o => o.Items.Sum(i => i.Quantity)),
                    ConversionRate = 0, // This needs to be calculated based on visitor data
                    RefundRate = g.Count(o => o.Status == OrderStatus.Refunded) * 100m / g.Count(),
                    ProfitMargin = 0, // This needs cost data to calculate
                    GrossProfit = 0, // This needs cost data to calculate
                    NetProfit = 0, // This needs cost data to calculate
                    TotalCost = 0, // This needs cost data to calculate
                    AverageItemPrice = g.Average(o => o.Items.Average(i => i.UnitPrice)),
                    AverageItemCost = 0, // This needs cost data to calculate
                    UniqueCustomers = g.Select(o => o.UserId).Distinct().Count(),
                    NewCustomers = 0, // This needs historical data to calculate
                    ReturningCustomers = 0, // This needs historical data to calculate
                    CustomerRetentionRate = 0, // This needs historical data to calculate
                    CustomerAcquisitionCost = 0, // This needs marketing data to calculate
                    CustomerLifetimeValue = 0 // This needs historical data to calculate
                })
                .OrderBy(ds => ds.Date)
                .ToListAsync();

            return dailySales;
        }

        public async Task<IEnumerable<CategorySales>> GetCategorySalesAsync()
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);

            var categorySales = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.PaymentStatus == PaymentStatus.Paid)
                .SelectMany(o => o.Items)
                .GroupBy(i => i.Product.Category.Name)
                .Select(g => new CategorySales
                {
                    Category = g.Key,
                    TotalOrders = g.Select(i => i.OrderId).Distinct().Count(),
                    TotalRevenue = g.Sum(i => i.Quantity * i.UnitPrice),
                    AverageOrderValue = g.Sum(i => i.Quantity * i.UnitPrice) / g.Select(i => i.OrderId).Distinct().Count(),
                    TotalItems = g.Sum(i => i.Quantity),
                    AverageItemPrice = g.Average(i => i.UnitPrice),
                    TotalCost = 0, // This needs cost data to calculate
                    GrossProfit = 0, // This needs cost data to calculate
                    NetProfit = 0, // This needs cost data to calculate
                    ProfitMargin = 0, // This needs cost data to calculate
                    ConversionRate = 0, // This needs visitor data to calculate
                    RefundRate = 0, // This needs refund data to calculate
                    UniqueCustomers = g.Select(i => i.Order.UserId).Distinct().Count(),
                    CustomerRetentionRate = 0, // This needs historical data to calculate
                    MarketShare = 0, // This needs total market data to calculate
                    YearOverYearGrowth = 0, // This needs historical data to calculate
                    StockLevel = 0, // This needs inventory data to calculate
                    InventoryTurnover = 0, // This needs inventory data to calculate
                    DaysOfInventory = 0 // This needs inventory data to calculate
                })
                .OrderByDescending(cs => cs.TotalRevenue)
                .ToListAsync();

            return categorySales;
        }

        public async Task<IEnumerable<PaymentMethodStats>> GetPaymentMethodStatsAsync()
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);

            var paymentStats = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .GroupBy(o => o.PaymentMethod)
                .Select(g => new PaymentMethodStats
                {
                    Method = g.Key,
                    TotalTransactions = g.Count(),
                    TotalAmount = g.Sum(o => o.Total),
                    AverageTransactionAmount = g.Average(o => o.Total),
                    RefundRate = g.Count(o => o.Status == OrderStatus.Refunded) * 100m / g.Count(),
                    ChargebackRate = 0, // This needs payment processor data to calculate
                    FailureRate = g.Count(o => o.PaymentStatus == PaymentStatus.Failed) * 100m / g.Count(),
                    ProcessingFees = 0, // This needs payment processor data to calculate
                    NetRevenue = g.Sum(o => o.Total), // This should subtract processing fees and refunds
                    UsagePercentage = 0, // This needs to be calculated relative to total orders
                    ConversionRate = 0, // This needs checkout attempt data to calculate
                    CustomerPreference = 0, // This needs customer preference data to calculate
                    FraudRate = 0, // This needs fraud detection data to calculate
                    AverageProcessingTime = 0, // This needs payment processing time data to calculate
                    SuccessRate = g.Count(o => o.PaymentStatus == PaymentStatus.Paid) * 100m / g.Count()
                })
                .OrderByDescending(ps => ps.TotalAmount)
                .ToListAsync();

            return paymentStats;
        }

        public async Task<IEnumerable<Product>> SearchAsync(string query)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive && (
                    p.Name.Contains(query) ||
                    p.Description.Contains(query) ||
                    p.Category.Name.Contains(query)
                ))
                .OrderByDescending(p => p.Rating)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Category.Name == category && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsFeatured && p.IsActive)
                .OrderByDescending(p => p.Rating)
                .ToListAsync();
        }

        public async Task<Product> DeleteAsync(Product product)
        {
            var productToDelete = await GetByIdAsync(product.Id);
            if (productToDelete == null)
            {
                throw new KeyNotFoundException($"Product with ID {product.Id} not found");
            }

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
            return productToDelete;
        }

        public async Task<IEnumerable<Product>> GetByVendorIdAsync(int vendorId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.OrderItems)
                .Where(p => p.VendorId == vendorId)
                .ToListAsync();
        }

        public IQueryable<Product> GetQueryable()
        {
            return _context.Products.AsQueryable();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByVendorAsync(int vendorId, int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .Where(p => p.VendorId == vendorId && p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Rating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetNewArrivalsAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetBestSellersAsync(int count)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int count)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return new List<Product>();

            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .Where(p => p.CategoryId == product.CategoryId && 
                           p.Id != productId && 
                           p.IsActive)
                .OrderByDescending(p => p.Rating)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm, int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Vendor)
                .Include(p => p.Reviews)
                .Where(p => p.IsActive && 
                           (p.Name.Contains(searchTerm) || 
                            p.Description.Contains(searchTerm)))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

    }
}