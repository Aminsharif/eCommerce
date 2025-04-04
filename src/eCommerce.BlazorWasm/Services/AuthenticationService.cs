using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using eCommerce.BlazorWasm.Services.Interfaces;
using eCommerce.Core.DTOs.Auth;
using eCommerce.Core.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;

namespace eCommerce.BlazorWasm.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        private readonly IUserRepository _userRepository;

        public AuthenticationService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
        }

        public async Task<AuthResult> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (authResponse != null)
                    {
                        var token = authResponse.Token;
                        await _localStorage.SetItemAsync("authToken", token);
                        ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(token);
                        return AuthResult.Success("Login successful");
                    }
                    return AuthResult.Failure("Invalid response from server");
                }
                var errorMessage = await response.Content.ReadAsStringAsync();
                return AuthResult.Failure(string.IsNullOrEmpty(errorMessage) ? "Login failed" : errorMessage);
            }
            catch (Exception ex)
            {
                return AuthResult.Failure("An error occurred during login");
            }
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (authResponse != null)
                    {
                        var token = authResponse.Token;
                        await _localStorage.SetItemAsync("authToken", token);
                        ((CustomAuthStateProvider)_authStateProvider).NotifyUserAuthentication(token);
                        return AuthResult.Success("Registration successful");
                    }
                    return AuthResult.Success("Registration successful, please login");
                }
                var errorMessage = await response.Content.ReadAsStringAsync();
                return AuthResult.Failure(string.IsNullOrEmpty(errorMessage) ? "Registration failed" : errorMessage);
            }
            catch (Exception ex)
            {
                return AuthResult.Failure("An error occurred during registration");
            }
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((CustomAuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<AuthResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/change-password", changePasswordDto);
                if (response.IsSuccessStatusCode)
                {
                    return AuthResult.Success("Password changed successfully");
                }
                var errorMessage = await response.Content.ReadAsStringAsync();
                return AuthResult.Failure(string.IsNullOrEmpty(errorMessage) ? "Password change failed" : errorMessage);
            }
            catch (Exception ex)
            {
                return AuthResult.Failure("An error occurred while changing password");
            }
        }

        public async Task<AuthResult> ForgotPasswordAsync(string email)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", new { Email = email });
                if (response.IsSuccessStatusCode)
                {
                    return AuthResult.Success("Password reset instructions sent to your email");
                }
                var errorMessage = await response.Content.ReadAsStringAsync();
                return AuthResult.Failure(string.IsNullOrEmpty(errorMessage) ? "Failed to process forgot password request" : errorMessage);
            }
            catch (Exception ex)
            {
                return AuthResult.Failure("An error occurred while processing forgot password request");
            }
        }

        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password", resetPasswordDto);
                if (response.IsSuccessStatusCode)
                {
                    return AuthResult.Success("Password has been reset successfully");
                }
                var errorMessage = await response.Content.ReadAsStringAsync();
                return AuthResult.Failure(string.IsNullOrEmpty(errorMessage) ? "Failed to reset password" : errorMessage);
            }
            catch (Exception ex)
            {
                return AuthResult.Failure("An error occurred while resetting password");
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.IsAuthenticated ?? false;
        }

        public async Task<string> GetUserNameAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.Identity?.Name ?? string.Empty;
        }

        public async Task<string> GetUserRolesAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
               
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return string.Empty;

            var roleClaims = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();

            return string.Join(",", roleClaims);
        }
    }
}