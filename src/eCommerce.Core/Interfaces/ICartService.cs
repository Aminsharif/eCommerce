using System.Threading.Tasks;
using eCommerce.Core.DTOs.Cart;

namespace eCommerce.Core.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(int userId);
        Task<CartDto> AddToCartAsync(int userId, AddToCartDto addToCartDto);
        Task<CartDto> UpdateCartItemAsync(int userId, UpdateCartItemDto updateCartItemDto);
        Task<CartDto> RemoveFromCartAsync(int userId, RemoveFromCartDto removeFromCartDto);
        Task<CartDto> ClearCartAsync(int userId);
        Task<decimal> GetCartTotalAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<bool> IsProductInCartAsync(int userId, int productId);
        Task<CartDto> MergeCartsAsync(int userId, string guestCartId);
        Task<CartDto> ApplyCouponAsync(int userId, string couponCode);
        Task<CartDto> RemoveCouponAsync(int userId);
        Task<CartDto> UpdateShippingAddressAsync(int userId, int addressId);
        Task<CartDto> UpdateShippingMethodAsync(int userId, string shippingMethod);
        Task<CartDto> UpdatePaymentMethodAsync(int userId, string paymentMethod);
        Task<CartDto> SaveCartForLaterAsync(int userId);
        Task<CartDto> RestoreSavedCartAsync(int userId);
        Task<bool> ValidateCartAsync(int userId);
    }
} 