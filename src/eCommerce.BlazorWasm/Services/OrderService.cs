using System.Net.Http.Json;
using eCommerce.BlazorWasm.Services.Interfaces;
using eCommerce.Core.DTOs.Order;

namespace eCommerce.BlazorWasm.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<OrderDto>> GetOrdersAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<OrderDto>>($"api/orders?page={page}&pageSize={pageSize}") ?? new List<OrderDto>();
            }
            catch
            {
                return new List<OrderDto>();
            }
        }

        public async Task<OrderDto> GetOrderByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrderDto>($"api/orders/{id}") ?? new OrderDto();
            }
            catch
            {
                return new OrderDto();
            }
        }

        public async Task<bool> CreateOrderAsync(CreateOrderDto orderDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/orders", orderDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/orders/{orderId}/status", new { Status = status });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/orders/{orderId}/cancel", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<OrderDto>> GetUserOrdersAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<OrderDto>>("api/orders/user") ?? new List<OrderDto>();
            }
            catch
            {
                return new List<OrderDto>();
            }
        }

        public async Task<OrderDto> GetLatestOrderAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrderDto>("api/orders/latest") ?? new OrderDto();
            }
            catch
            {
                return new OrderDto();
            }
        }

        public async Task<bool> ProcessPaymentAsync(int orderId, PaymentDto paymentDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"api/orders/{orderId}/payment", paymentDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<OrderDto>> SearchOrdersAsync(string searchTerm)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<OrderDto>>($"api/orders/search?term={searchTerm}") ?? new List<OrderDto>();
            }
            catch
            {
                return new List<OrderDto>();
            }
        }

        public async Task<OrderSummaryDto> GetOrderSummaryAsync(int orderId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<OrderSummaryDto>($"api/orders/{orderId}/summary") ?? new OrderSummaryDto();
            }
            catch
            {
                return new OrderSummaryDto();
            }
        }
    }
}