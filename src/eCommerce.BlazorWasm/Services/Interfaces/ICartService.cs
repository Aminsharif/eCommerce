using eCommerce.Core.DTOs.Cart;

namespace eCommerce.BlazorWasm.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync();
        Task<bool> AddToCartAsync(int productId, int quantity);
        Task<bool> UpdateCartItemAsync(int productId, int quantity);
        Task<bool> RemoveFromCartAsync(int productId);
        Task<bool> ClearCartAsync();
        Task<int> GetCartItemCountAsync();
        Task<decimal> GetCartTotalAsync();
    }
}