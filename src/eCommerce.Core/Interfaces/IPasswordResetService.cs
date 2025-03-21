using System.Threading.Tasks;

namespace eCommerce.Core.Interfaces
{
    public interface IPasswordResetService
    {
        Task<string> GeneratePasswordResetTokenAsync(string email);
        Task<bool> ValidatePasswordResetTokenAsync(string email, string token);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
        Task<bool> SendPasswordResetEmailAsync(string email, string resetLink);
    }
} 