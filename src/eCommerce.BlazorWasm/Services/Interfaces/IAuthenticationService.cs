using eCommerce.Core.DTOs.Auth;

namespace eCommerce.BlazorWasm.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthResult> LoginAsync(LoginDto loginDto);
        Task<AuthResult> RegisterAsync(RegisterDto registerDto);
        Task LogoutAsync();
        Task<AuthResult> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<AuthResult> ForgotPasswordAsync(string email);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> IsAuthenticatedAsync();
        Task<string> GetUserNameAsync();
        Task<string> GetUserRolesAsync();
    }
}