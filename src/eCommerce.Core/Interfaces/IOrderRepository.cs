using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<Order> GetOrderByOrderNumber(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByUser(int userId);
        Task<IEnumerable<Order>> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByDateAsync(DateTime date);
        Task UpdateOrderStatus(int orderId, OrderStatus status);
        Task UpdatePaymentStatus(int orderId, PaymentStatus status);
        Task<decimal> GetTotalRevenue(DateTime startDate, DateTime endDate);
        Task<int> GetTotalOrders(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetRecentOrdersAsync(int count);
        Task<IEnumerable<Order>> GetOrdersAsync(int page, int pageSize);
        Task<int> GetTotalCountAsync();
        Task<IEnumerable<DailySales>> GetDailySalesAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<CategorySales>> GetCategorySalesAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<PaymentMethodStats>> GetPaymentMethodStatsAsync(DateTime startDate, DateTime endDate);
    }
} 