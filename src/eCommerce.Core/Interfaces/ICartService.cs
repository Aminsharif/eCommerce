using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCart(int userId);
        Task<Cart> AddToCart(int userId, int productId, int quantity);
        Task<Cart> UpdateCartItem(int userId, int productId, int quantity);
        Task<Cart> RemoveFromCart(int userId, int productId);
        Task<Cart> ClearCart(int userId);
        Task<Cart> ApplyCoupon(int userId, string couponCode);
        Task<Cart> RemoveCoupon(int userId);
        Task<bool> ValidateCoupon(string couponCode);
        Task<decimal> CalculateCartTotal(int userId);
        Task<bool> TransferCartToOrder(int userId);
        Task<bool> ValidateCartItems(int userId);
        Task<bool> UpdateCartPrices(int userId);
        Task<int> GetCartItemCount(int userId);
        Task<bool> MergeAnonymousCart(string anonymousCartId, int userId);
    }
} 