using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Cart?> GetCartBySessionIdAsync(string sessionId);
        Task<CartItem> GetCartItemAsync(int cartId, int productId);
        Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId);
        Task<Cart> AddItemToCartAsync(int cartId, CartItem cartItem);
        Task<Cart> UpdateCartItemAsync(int cartId, CartItem cartItem);
        Task<Cart> RemoveItemFromCartAsync(int cartId, int cartItemId);
        Task<Cart> ClearCartAsync(int cartId);
        Task<Cart> UpdateCartTotalsAsync(int cartId);
        Task<Cart> ApplyCouponAsync(int cartId, string couponCode);
        Task<Cart> RemoveCouponAsync(int cartId);
        Task<Cart> UpdateShippingAddressAsync(int cartId, int addressId);
        Task<Cart> UpdateShippingMethodAsync(int cartId, string shippingMethod);
        Task<Cart> UpdatePaymentMethodAsync(int cartId, string paymentMethod);
        Task<Cart> SaveCartForLaterAsync(int cartId);
        Task<Cart> RestoreSavedCartAsync(int cartId);
        Task<Cart> MergeCartsAsync(int sourceCartId, int targetCartId);
        Task DeleteExpiredCartsAsync();
        Task<bool> IsProductInCartAsync(int cartId, int productId);
    }
} 