using System.Net.Http.Json;
using eCommerce.BlazorWasm.Services.Interfaces;
using eCommerce.Core.DTOs.Auth;
using eCommerce.Core.DTOs.Address;
using eCommerce.Core.DTOs.Wishlist;
using eCommerce.Core.DTOs.Notification;

namespace eCommerce.BlazorWasm.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserDto>("api/users/current") ?? new UserDto();
            }
            catch
            {
                return new UserDto();
            }
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<UserDto>($"api/users/{id}") ?? new UserDto();
            }
            catch
            {
                return new UserDto();
            }
        }

        public async Task<bool> UpdateProfileAsync(UpdateUserProfileDto profileDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/users/profile", profileDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<AddressDto>> GetUserAddressesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<AddressDto>>("api/users/addresses") ?? new List<AddressDto>();
            }
            catch
            {
                return new List<AddressDto>();
            }
        }

        public async Task<bool> AddAddressAsync(AddressDto AddressDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/users/addresses", AddressDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAddressAsync(int addressId, AddressDto AddressDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/users/addresses/{addressId}", AddressDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAddressAsync(int addressId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/users/addresses/{addressId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetDefaultAddressAsync(int addressId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/users/addresses/{addressId}/default", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<WishlistDto>> GetWishlistAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<WishlistDto>>("api/users/wishlist") ?? new List<WishlistDto>();
            }
            catch
            {
                return new List<WishlistDto>();
            }
        }

        public async Task<bool> AddToWishlistAsync(int productId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/users/wishlist/{productId}", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(int productId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/users/wishlist/{productId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateNotificationPreferencesAsync(NotificationPreferencesDto preferencesDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync("api/users/notifications", preferencesDto);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}