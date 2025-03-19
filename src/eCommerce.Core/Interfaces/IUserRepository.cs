using System.Collections.Generic;
using System.Threading.Tasks;
using eCommerce.Core.Models;

namespace eCommerce.Core.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
        Task<bool> IsEmailAvailableAsync(string email);
        Task<User?> GetByCredentialsAsync(string email, string passwordHash);
        Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
        Task<IEnumerable<Review>> GetUserReviewsAsync(int userId);
        Task<IEnumerable<Product>> GetWishListAsync(int userId);
        Task AddToWishListAsync(int userId, int productId);
        Task RemoveFromWishListAsync(int userId, int productId);
        Task<User?> GetUserWithAddressesAsync(int userId);
        Task<User> GetUserByEmail(string email);
        Task<bool> IsEmailUnique(string email);
        Task<int> GetTotalCountAsync();
        Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize);
    }
} 