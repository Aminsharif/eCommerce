using eCommerce.Core.DTOs.Order;

namespace eCommerce.BlazorWasm.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderDto>> GetOrdersAsync(int page = 1, int pageSize = 10);
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<bool> CreateOrderAsync(CreateOrderDto orderDto);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
        Task<bool> CancelOrderAsync(int orderId);
        Task<List<OrderDto>> GetUserOrdersAsync();
        Task<OrderDto> GetLatestOrderAsync();
        Task<bool> ProcessPaymentAsync(int orderId, PaymentDto paymentDto);
        Task<List<OrderDto>> SearchOrdersAsync(string searchTerm);
        Task<OrderSummaryDto> GetOrderSummaryAsync(int orderId);
    }
}