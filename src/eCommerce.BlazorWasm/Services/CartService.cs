using System.Net.Http.Json;
using eCommerce.BlazorWasm.Services.Interfaces;
using eCommerce.Core.DTOs.Cart;
using eCommerce.Core.DTOs.Settings;

namespace eCommerce.BlazorWasm.Services
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;

        public CartService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CartDto> GetCartAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CartDto>("api/cart") ?? new CartDto();
            }
            catch
            {
                return new CartDto();
            }
        }

        public async Task<bool> AddToCartAsync(int productId, int quantity)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/cart", new { ProductId = productId, Quantity = quantity });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCartItemAsync(int productId, int quantity)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/cart/{productId}", new { Quantity = quantity });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int productId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/cart/{productId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ClearCartAsync()
        {
            try
            {
                var response = await _httpClient.DeleteAsync("api/cart");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetCartItemCountAsync()
        {
            try
            {
                var cart = await GetCartAsync();
                return cart.Items?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task<decimal> GetCartTotalAsync()
        {
            try
            {
                var cart = await GetCartAsync();
                return cart.Total;
            }
            catch
            {
                return 0;
            }
        }
    }
}