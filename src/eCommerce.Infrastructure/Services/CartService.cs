using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using eCommerce.Core.DTOs.Cart;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;

namespace eCommerce.Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        private async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepository.AddAsync(cart);
            }
            return cart;
        }

        public async Task<CartDto> GetCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> AddToCartAsync(int userId, AddToCartDto addToCartDto)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var product = await _productRepository.GetByIdAsync(addToCartDto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                ProductImage = product.ImagesJson,
                Quantity = addToCartDto.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * addToCartDto.Quantity
            };

            cart = await _cartRepository.AddItemToCartAsync(cart.Id, cartItem);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> UpdateCartItemAsync(int userId, UpdateCartItemDto updateCartItemDto)
        {
            var cart = await GetOrCreateCartAsync(userId);

            var cartItem = cart.Items.FirstOrDefault(i => i.Id == updateCartItemDto.CartItemId);
            if (cartItem == null)
                throw new Exception("Cart item not found");

            cartItem.Quantity = updateCartItemDto.Quantity;
            cartItem.TotalPrice = cartItem.UnitPrice * updateCartItemDto.Quantity;

            cart = await _cartRepository.UpdateCartItemAsync(cart.Id, cartItem);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> RemoveFromCartAsync(int userId, RemoveFromCartDto removeFromCartDto)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart = await _cartRepository.RemoveItemFromCartAsync(cart.Id, removeFromCartDto.CartItemId);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> ClearCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            await _cartRepository.ClearCartAsync(cart.Id);
            cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return cart?.Total ?? 0;
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            return cart?.Items.Sum(i => i.Quantity) ?? 0;
        }

        public async Task<bool> IsProductInCartAsync(int userId, int productId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                return false;

            return await _cartRepository.IsProductInCartAsync(cart.Id, productId);
        }

        public async Task<CartDto> MergeCartsAsync(int userId, string guestCartId)
        {
            var userCart = await GetOrCreateCartAsync(userId);
            var guestCart = await _cartRepository.GetCartBySessionIdAsync(guestCartId);

            if (guestCart == null)
                throw new Exception("Guest cart not found");

            var mergedCart = await _cartRepository.MergeCartsAsync(guestCart.Id, userCart.Id);
            return _mapper.Map<CartDto>(mergedCart);
        }

        public async Task<CartDto> ApplyCouponAsync(int userId, string couponCode)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart = await _cartRepository.ApplyCouponAsync(cart.Id, couponCode);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> RemoveCouponAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart = await _cartRepository.RemoveCouponAsync(cart.Id);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> UpdateShippingAddressAsync(int userId, int addressId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart = await _cartRepository.UpdateShippingAddressAsync(cart.Id, addressId);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> UpdateShippingMethodAsync(int userId, string shippingMethod)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart = await _cartRepository.UpdateShippingMethodAsync(cart.Id, shippingMethod);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> UpdatePaymentMethodAsync(int userId, string paymentMethod)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart = await _cartRepository.UpdatePaymentMethodAsync(cart.Id, paymentMethod);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> SaveCartForLaterAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart = await _cartRepository.SaveCartForLaterAsync(cart.Id);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> RestoreSavedCartAsync(int userId)
        {
            var cart = await GetOrCreateCartAsync(userId);
            cart = await _cartRepository.RestoreSavedCartAsync(cart.Id);
            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> ValidateCartAsync(int userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                return false;

            foreach (var item in cart.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null || !product.IsActive || product.StockQuantity < item.Quantity)
                    return false;
            }

            return true;
        }
    }
} 