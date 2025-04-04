using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserById(int id);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmail(string email);
        Task<IEnumerable<User>> GetUsersByRole(UserRole role);
        Task<User> CreateUser(User user);
        Task<User> CreateUserAsync(User user);
        Task<User> AuthenticateUser(string email, string password);
        Task<bool> DeactivateUser(int id);
        Task<bool> AddAddress(int userId, Address address);
        Task<bool> UpdateAddress(int userId, Address address);
        Task<bool> DeleteAddress(int userId, int addressId);
        Task<IEnumerable<Order>> GetUserOrders(int userId);
        Task<IEnumerable<Review>> GetUserReviews(int userId);
        Task<bool> ResetPassword(string email);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);
        Task<bool> IsEmailAvailable(string email);
        Task<User> RegisterUser(User user, string password);
        Task<IEnumerable<Product>> GetWishlist(int userId);
        Task<bool> ValidateEmail(string email);
        Task<int> GetTotalCustomersCountAsync();
    }
} 