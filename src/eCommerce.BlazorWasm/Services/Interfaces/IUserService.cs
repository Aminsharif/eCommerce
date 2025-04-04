using eCommerce.Core.DTOs.Auth;
using eCommerce.Core.DTOs.Address;
using eCommerce.Core.DTOs.Wishlist;
using eCommerce.Core.DTOs.Notification;

namespace eCommerce.BlazorWasm.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetCurrentUserAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<bool> UpdateProfileAsync(UpdateUserProfileDto profileDto);
        Task<List<AddressDto>> GetUserAddressesAsync();
        Task<bool> AddAddressAsync(AddressDto addressDto);
        Task<bool> UpdateAddressAsync(int addressId, AddressDto addressDto);
        Task<bool> DeleteAddressAsync(int addressId);
        Task<bool> SetDefaultAddressAsync(int addressId);
        Task<List<WishlistDto>> GetWishlistAsync();
        Task<bool> AddToWishlistAsync(int productId);
        Task<bool> RemoveFromWishlistAsync(int productId);
        Task<bool> UpdateNotificationPreferencesAsync(NotificationPreferencesDto preferencesDto);
    }
}