using System;
using System.Security.Claims;
using System.Threading.Tasks;
using eCommerce.Core.DTOs.Cart;
using eCommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User not authenticated");

            return int.Parse(userIdClaim.Value);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        [Authorize]
        public async Task<ActionResult<CartDto>> AddToCart([FromBody] AddToCartDto addToCartDto)
        {
            var userId = GetUserId();
            var cart = await _cartService.AddToCartAsync(userId, addToCartDto);
            return Ok(cart);
        }

        [HttpPut("items")]
        [Authorize]
        public async Task<ActionResult<CartDto>> UpdateCartItem([FromBody] UpdateCartItemDto updateCartItemDto)
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdateCartItemAsync(userId, updateCartItemDto);
            return Ok(cart);
        }

        [HttpDelete("items/{cartItemId}")]
        [Authorize]
        public async Task<ActionResult<CartDto>> RemoveFromCart(int cartItemId)
        {
            var userId = GetUserId();
            var removeFromCartDto = new RemoveFromCartDto { CartItemId = cartItemId };
            var cart = await _cartService.RemoveFromCartAsync(userId, removeFromCartDto);
            return Ok(cart);
        }

        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<CartDto>> ClearCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.ClearCartAsync(userId);
            return Ok(cart);
        }

        [HttpGet("total")]
        [Authorize]
        public async Task<ActionResult<decimal>> GetCartTotal()
        {
            var userId = GetUserId();
            var total = await _cartService.GetCartTotalAsync(userId);
            return Ok(total);
        }

        [HttpGet("count")]
        [Authorize]
        public async Task<ActionResult<int>> GetCartItemCount()
        {
            var userId = GetUserId();
            var count = await _cartService.GetCartItemCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("check-product/{productId}")]
        [Authorize]
        public async Task<ActionResult<bool>> IsProductInCart(int productId)
        {
            var userId = GetUserId();
            var isInCart = await _cartService.IsProductInCartAsync(userId, productId);
            return Ok(isInCart);
        }

        [HttpPost("merge/{guestCartId}")]
        [Authorize]
        public async Task<ActionResult<CartDto>> MergeCarts(string guestCartId)
        {
            var userId = GetUserId();
            var cart = await _cartService.MergeCartsAsync(userId, guestCartId);
            return Ok(cart);
        }

        [HttpPost("coupon/{couponCode}")]
        [Authorize]
        public async Task<ActionResult<CartDto>> ApplyCoupon(string couponCode)
        {
            var userId = GetUserId();
            var cart = await _cartService.ApplyCouponAsync(userId, couponCode);
            return Ok(cart);
        }

        [HttpDelete("coupon")]
        [Authorize]
        public async Task<ActionResult<CartDto>> RemoveCoupon()
        {
            var userId = GetUserId();
            var cart = await _cartService.RemoveCouponAsync(userId);
            return Ok(cart);
        }

        [HttpPut("shipping-address/{addressId}")]
        [Authorize]
        public async Task<ActionResult<CartDto>> UpdateShippingAddress(int addressId)
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdateShippingAddressAsync(userId, addressId);
            return Ok(cart);
        }

        [HttpPut("shipping-method/{method}")]
        [Authorize]
        public async Task<ActionResult<CartDto>> UpdateShippingMethod(string method)
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdateShippingMethodAsync(userId, method);
            return Ok(cart);
        }

        [HttpPut("payment-method/{method}")]
        [Authorize]
        public async Task<ActionResult<CartDto>> UpdatePaymentMethod(string method)
        {
            var userId = GetUserId();
            var cart = await _cartService.UpdatePaymentMethodAsync(userId, method);
            return Ok(cart);
        }

        [HttpPost("save-for-later")]
        [Authorize]
        public async Task<ActionResult<CartDto>> SaveCartForLater()
        {
            var userId = GetUserId();
            var cart = await _cartService.SaveCartForLaterAsync(userId);
            return Ok(cart);
        }

        [HttpPost("restore-saved")]
        [Authorize]
        public async Task<ActionResult<CartDto>> RestoreSavedCart()
        {
            var userId = GetUserId();
            var cart = await _cartService.RestoreSavedCartAsync(userId);
            return Ok(cart);
        }

        [HttpGet("validate")]
        [Authorize]
        public async Task<ActionResult<bool>> ValidateCart()
        {
            var userId = GetUserId();
            var isValid = await _cartService.ValidateCartAsync(userId);
            return Ok(isValid);
        }
    }
} 