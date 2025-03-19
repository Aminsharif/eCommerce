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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Orders)
                .Include(u => u.Reviews)
                .Include(u => u.WishList)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User?> GetByCredentialsAsync(string email, string passwordHash)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == passwordHash);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
                .Include(u => u.Orders)
                .ThenInclude(o => o.ShippingAddress)
                .Include(u => u.Orders)
                .ThenInclude(o => o.BillingAddress)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Orders ?? new List<Order>();
        }

        public async Task<IEnumerable<Review>> GetUserReviewsAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Reviews)
                .ThenInclude(r => r.Product)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.Reviews ?? new List<Review>();
        }

        public async Task<IEnumerable<Product>> GetWishListAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.WishList)
                .FirstOrDefaultAsync(u => u.Id == userId);

            return user?.WishList ?? new List<Product>();
        }

        public async Task AddToWishListAsync(int userId, int productId)
        {
            var user = await _context.Users
                .Include(u => u.WishList)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {productId} not found");

            if (!user.WishList.Any(p => p.Id == productId))
            {
                user.WishList.Add(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromWishListAsync(int userId, int productId)
        {
            var user = await _context.Users
                .Include(u => u.WishList)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var product = user.WishList.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                user.WishList.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetUserWithAddressesAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> IsEmailUnique(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRole(UserRole role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public async Task UpdateUserStatus(int userId, bool isActive)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = isActive;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidateCredentials(string email, string passwordHash)
        {
            var user = await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
            return user != null && user.PasswordHash == passwordHash && user.IsActive;
        }

        public async Task UpdateLastLogin(int userId)
        {
            var user = await _dbSet.FindAsync(userId);
            if (user != null)
            {
                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetByIdWithReviewsAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Reviews)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<IEnumerable<User>> GetUsersAsync(int page, int pageSize)
        {
            return await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new List<string>();
            }

            return new List<string> { user.Role.ToString() }; // Convert enum to string and return as list
        }
        public async Task<bool> IsInRoleAsync(int userId, UserRole role)
        {
            var user = await GetByIdAsync(userId);
            return user?.Role == role;
        }
    }
} 