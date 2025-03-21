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
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private new readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllActiveCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryWithProductsAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductCountAsync()
        {
            return await _context.Categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ParentId = c.ParentId,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    ProductCount = c.Products.Count(p => p.IsActive)
                })
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string name)
        {
            return !await _context.Categories.AnyAsync(c => c.Name == name);
        }

        public async Task<IEnumerable<Category>> GetParentCategoriesAsync()
        {
            return await _context.Categories
                .Where(c => c.ParentId == null && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId)
        {
            return await _context.Categories
                .Where(c => c.ParentId == parentId && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<int> GetProductCountAsync(int categoryId)
        {
            return await _context.Products
                .CountAsync(p => p.CategoryId == categoryId && p.IsActive);
        }

        public async Task<decimal> GetAverageProductPriceAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .AverageAsync(p => p.Price);
        }

        public async Task<decimal> GetTotalRevenueAsync(int categoryId)
        {
            return await _context.OrderItems
                .Where(oi => oi.Product.CategoryId == categoryId && 
                            oi.Order.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice);
        }

        public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
        {
            return await _context.Categories
                .Where(c => c.IsActive && 
                           (c.Name.Contains(searchTerm) || 
                            c.Description.Contains(searchTerm)))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> HasActiveProductsAsync(int categoryId)
        {
            return await _context.Products
                .AnyAsync(p => p.CategoryId == categoryId && p.IsActive);
        }

        public async Task<IEnumerable<Category>> GetFeaturedCategoriesAsync(int count)
        {
            return await _context.Categories
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.Products.Count(p => p.IsActive))
                .Take(count)
                .ToListAsync();
        }

        public async Task<CategoryStatistics> GetCategoryStatisticsAsync(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId && p.IsActive)
                .ToListAsync();

            var orderItems = await _context.OrderItems
                .Where(oi => oi.Product.CategoryId == categoryId && 
                            oi.Order.PaymentStatus == PaymentStatus.Paid)
                .ToListAsync();

            return new CategoryStatistics
            {
                TotalProducts = products.Count,
                ActiveProducts = products.Count(p => p.IsActive),
                TotalRevenue = orderItems.Sum(oi => oi.Quantity * oi.UnitPrice),
                AverageProductPrice = products.Any() ? products.Average(p => p.Price) : 0,
                TotalOrders = orderItems.Select(oi => oi.OrderId).Distinct().Count(),
                TotalItemsSold = orderItems.Sum(oi => oi.Quantity),
                AverageOrderValue = orderItems.Any() 
                    ? orderItems.Sum(oi => oi.Quantity * oi.UnitPrice) / 
                      orderItems.Select(oi => oi.OrderId).Distinct().Count() 
                    : 0,
                LowStockProducts = products.Count(p => p.StockQuantity <= p.ReorderPoint),
                OutOfStockProducts = products.Count(p => p.StockQuantity == 0),
                TotalInventoryValue = products.Sum(p => p.Price * p.StockQuantity),
                AverageRating = products.Any() ? products.Average(p => p.Rating) : 0,
                TotalReviews = products.Sum(p => p.Reviews.Count),
                ConversionRate = 0, // This needs visitor data to calculate
                ReturnRate = 0, // This needs return data to calculate
                ProfitMargin = 0, // This needs cost data to calculate
                YearOverYearGrowth = 0, // This needs historical data to calculate
                MarketShare = 0, // This needs total market data to calculate
                CustomerSatisfaction = products.Any() ? products.Average(p => p.Rating) : 0,
                InventoryTurnover = 0, // This needs historical data to calculate
                DaysOfInventory = 0 // This needs historical data to calculate
            };
        }

        public override async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
} 