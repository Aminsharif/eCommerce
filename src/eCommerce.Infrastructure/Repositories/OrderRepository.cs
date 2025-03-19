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
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUser(int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<IEnumerable<Order>> GetRecentOrdersAsync(int count)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(int page, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = await _dbSet.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePaymentStatus(int orderId, PaymentStatus paymentStatus)
        {
            var order = await _dbSet.FindAsync(orderId);
            if (order != null)
            {
                order.PaymentStatus = paymentStatus;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetTotalRevenue(DateTime startDate, DateTime endDate)
        {
            var orders = await _dbSet
                .Where(o => o.CreatedAt >= startDate && 
                           o.CreatedAt <= endDate && 
                           o.PaymentStatus == PaymentStatus.Paid)
                .ToListAsync();

            return orders.Sum(o => o.Total);
        }

        public async Task<int> GetTotalOrders(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .CountAsync(o => o.CreatedAt >= startDate && o.CreatedAt <= endDate);
        }

        public override async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> GetOrderByOrderNumber(string orderNumber)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Payments)
                .Include(o => o.Commissions)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                throw new KeyNotFoundException($"Order with number {orderNumber} not found");

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .Where(o => o.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByPaymentStatusAsync(PaymentStatus status)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .Where(o => o.PaymentStatus == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByVendorIdAsync(int vendorId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .Where(o => o.Items.Any(i => i.Product.VendorId == vendorId))
                .ToListAsync();
        }

        public async Task<IEnumerable<DailySales>> GetDailySalesAsync(DateTime startDate, DateTime endDate)
        {
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

        public async Task<IEnumerable<CategorySales>> GetCategorySalesAsync(DateTime startDate, DateTime endDate)
        {
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

        public async Task<IEnumerable<PaymentMethodStats>> GetPaymentMethodStatsAsync(DateTime startDate, DateTime endDate)
        {
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

        public async Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date)
        {
            return await _context.Orders
                .Where(o => o.CreatedAt.Date == date.Date)
                .ToListAsync();
        }
    }
} 