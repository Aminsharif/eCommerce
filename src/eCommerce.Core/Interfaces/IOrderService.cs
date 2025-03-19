using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(Order order);
        Task<Order> GetOrderById(int id);
        Task<Order> GetOrderByNumber(string orderNumber);
        Task<IEnumerable<Order>> GetUserOrders(int userId);
        Task<bool> UpdateOrderStatus(int orderId, OrderStatus status);
        Task<bool> CancelOrder(int orderId);
        Task<bool> ProcessPayment(int orderId, PaymentMethodType paymentMethod, string transactionId);
        Task<bool> UpdateShippingInfo(int orderId, string trackingNumber);
        Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status);
        Task<IEnumerable<Order>> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
        Task<decimal> CalculateOrderTotal(IEnumerable<OrderItem> items);
        Task<bool> ValidateOrder(Order order);
        Task<IEnumerable<OrderItem>> GetOrderItems(int orderId);
        Task<Dictionary<string, decimal>> GetSalesReport(DateTime startDate, DateTime endDate);
        Task<bool> UpdatePaymentStatus(int orderId, PaymentStatus paymentStatus);
        Task<decimal> GetTotalRevenue(DateTime startDate, DateTime endDate);
        Task<int> GetTotalOrders(DateTime startDate, DateTime endDate);
        Task<int> GetTotalOrdersCount();
        Task<IEnumerable<Order>> GetRecentOrders(int count);
        Task<IEnumerable<Order>> GetOrders(int page, int pageSize);
        Task<Analytics> GetOrderAnalytics(DateTime startDate, DateTime endDate);
        Task<DailySales> GetDailySalesAsync(DateTime date);
        Task<ICollection<CategorySales>> GetCategorySalesAsync(DateTime startDate, DateTime endDate);
        Task<ICollection<PaymentMethodStats>> GetPaymentMethodStatsAsync(DateTime startDate, DateTime endDate);
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> UpdateOrderStatusAsync(int orderId, OrderStatus status);
        Task<Order> UpdatePaymentStatusAsync(int orderId, PaymentStatus status);
        Task<Analytics> GetOrderAnalyticsAsync(DateTime startDate, DateTime endDate);
    }
} 