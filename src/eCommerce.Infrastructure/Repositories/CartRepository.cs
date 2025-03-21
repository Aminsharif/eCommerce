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
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .Include(c => c.ShippingAddress)
                .FirstOrDefaultAsync(c => c.UserId == userId && !c.IsSavedForLater);
        }

        public async Task<Cart?> GetCartBySessionIdAsync(string sessionId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .Include(c => c.ShippingAddress)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);
        }

        public async Task<CartItem> GetCartItemAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsAsync(int cartId)
        {
            return await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
        }

        public async Task<Cart> AddItemToCartAsync(int cartId, CartItem cartItem)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += cartItem.Quantity;
                existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
            }
            else
            {
                cart.Items.Add(cartItem);
            }

            await UpdateCartTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateCartItemAsync(int cartId, CartItem cartItem)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            var existingItem = cart.Items.FirstOrDefault(i => i.Id == cartItem.Id)
                ?? throw new Exception($"Cart item with ID {cartItem.Id} not found");

            existingItem.Quantity = cartItem.Quantity;
            existingItem.TotalPrice = existingItem.UnitPrice * cartItem.Quantity;

            await UpdateCartTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> RemoveItemFromCartAsync(int cartId, int cartItemId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            var cartItem = cart.Items.FirstOrDefault(i => i.Id == cartItemId)
                ?? throw new Exception($"Cart item with ID {cartItemId} not found");

            cart.Items.Remove(cartItem);
            await UpdateCartTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> ClearCartAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.Items.Clear();
            await UpdateCartTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> IsProductInCartAsync(int cartId, int productId)
        {
            return await _context.CartItems
                .AnyAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
        }

        public async Task<Cart> MergeCartsAsync(int sourceCartId, int targetCartId)
        {
            var sourceCart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == sourceCartId)
                ?? throw new Exception($"Source cart with ID {sourceCartId} not found");

            var targetCart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == targetCartId)
                ?? throw new Exception($"Target cart with ID {targetCartId} not found");

            foreach (var item in sourceCart.Items)
            {
                var existingItem = targetCart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += item.Quantity;
                    existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                }
                else
                {
                    targetCart.Items.Add(new CartItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        ProductImage = item.ProductImage,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    });
                }
            }

            _context.Carts.Remove(sourceCart);
            await UpdateCartTotalsAsync(targetCartId);
            await _context.SaveChangesAsync();
            return targetCart;
        }

        public async Task<Cart> ApplyCouponAsync(int cartId, string couponCode)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.CouponCode = couponCode;
            // Note: Actual coupon validation and discount calculation should be handled by a separate service
            await UpdateCartTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> RemoveCouponAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.CouponCode = null;
            cart.Discount = 0;
            await UpdateCartTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateShippingAddressAsync(int cartId, int addressId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.ShippingAddressId = addressId;
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateShippingMethodAsync(int cartId, string shippingMethod)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.ShippingMethod = shippingMethod;
            // Note: Actual shipping cost calculation should be handled by a separate service
            await UpdateCartTotalsAsync(cartId);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdatePaymentMethodAsync(int cartId, string paymentMethod)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.PaymentMethod = paymentMethod;
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> SaveCartForLaterAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.IsSavedForLater = true;
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> RestoreSavedCartAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.IsSavedForLater = false;
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task DeleteExpiredCartsAsync()
        {
            var expirationDate = DateTime.UtcNow.AddDays(-30); // Carts older than 30 days
            var expiredCarts = await _context.Carts
                .Where(c => c.CreatedAt < expirationDate && c.SessionId != null)
                .ToListAsync();

            _context.Carts.RemoveRange(expiredCarts);
            await _context.SaveChangesAsync();
        }

        private async Task<Cart> UpdateCartTotalsAsync(int cartId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == cartId)
                ?? throw new Exception($"Cart with ID {cartId} not found");

            cart.SubTotal = cart.Items.Sum(i => i.TotalPrice);
            cart.Tax = cart.SubTotal * 0.1m; // 10% tax rate - this should be configurable
            // Note: Shipping cost and discount calculations should be handled by separate services
            cart.Total = cart.SubTotal + cart.Tax + cart.ShippingCost - cart.Discount;

            await _context.SaveChangesAsync();
            return cart;
        }

        Task<Cart> ICartRepository.UpdateCartTotalsAsync(int cartId)
        {
            throw new NotImplementedException();
        }
    }
} 