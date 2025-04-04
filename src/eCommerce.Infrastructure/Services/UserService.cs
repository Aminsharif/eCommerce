using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace eCommerce.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<UserService> _logger;

        public async Task<int> GetTotalCustomersCountAsync()
        {
            try
            {
                return await _userRepository.CountAsync(u => true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total customers count");
                throw;
            }
        }

        public UserService(IUserRepository userRepository, IOrderRepository orderRepository, IReviewRepository reviewRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetUsersByRole(UserRole role)
        {
            try
            {
                return await _userRepository.GetByRoleAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting users by role {role}");
                throw;
            }
        }

        public async Task<bool> IsEmailAvailable(string email)
        {
            return await _userRepository.IsEmailAvailableAsync(email);
        }

        public async Task<User> RegisterUser(User user, string password)
        {
            if (!await IsEmailAvailable(user.Email))
            {
                throw new InvalidOperationException("Email is already in use");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = hashedPassword;
            return await _userRepository.AddAsync(user);
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }
            return user;
        }

        public async Task<bool> DeactivateUser(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> ResetPassword(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            // In a real application, this would generate a reset token and send an email
            // For now, we'll just set a temporary password
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 8);
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(tempPassword);
            user.PasswordHash = hashedPassword;
            await _userRepository.UpdateAsync(user);

            return true;
        }

        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                return false;
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.PasswordHash = hashedPassword;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<IEnumerable<Order>> GetUserOrders(int userId)
        {
            return await _userRepository.GetUserOrdersAsync(userId);
        }

        public async Task<IEnumerable<Review>> GetUserReviews(int userId)
        {
            return await _userRepository.GetUserReviewsAsync(userId);
        }

        public async Task<bool> AddAddress(int userId, Address address)
        {
            var user = await _userRepository.GetUserWithAddressesAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Addresses.Add(address);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateAddress(int userId, Address address)
        {
            var user = await _userRepository.GetUserWithAddressesAsync(userId);
            if (user == null)
            {
                return false;
            }

            var existingAddress = user.Addresses.FirstOrDefault(a => a.Id == address.Id);
            if (existingAddress == null)
            {
                return false;
            }

            existingAddress.Street = address.Street;
            existingAddress.City = address.City;
            existingAddress.State = address.State;
            existingAddress.Country = address.Country;
            existingAddress.PostalCode = address.PostalCode;
            existingAddress.IsDefault = address.IsDefault;
            existingAddress.AddressType = address.AddressType;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeleteAddress(int userId, int addressId)
        {
            var user = await _userRepository.GetUserWithAddressesAsync(userId);
            if (user == null)
            {
                return false;
            }

            var address = user.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
            {
                return false;
            }

            user.Addresses.Remove(address);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(user.Id);
                if (existingUser == null)
                {
                    return false;
                }

                if (user.Email != existingUser.Email && !await IsEmailAvailable(user.Email))
                {
                    return false;
                }

                user.PasswordHash = existingUser.PasswordHash; // Preserve existing password
                await _userRepository.UpdateAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user {user.Id}");
                return false;
            }
        }

        public async Task<bool> AddToWishlist(int userId, int productId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            await _userRepository.AddToWishListAsync(userId, productId);
            return true;
        }

        public async Task<bool> RemoveFromWishlist(int userId, int productId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            await _userRepository.RemoveFromWishListAsync(userId, productId);
            return true;
        }

        public async Task<IEnumerable<Product>> GetWishlist(int userId)
        {
            return await _userRepository.GetWishListAsync(userId);
        }

        public async Task<bool> ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            // Basic email format validation
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return await Task.FromResult(addr.Address == email);
            }
            catch
            {
                return false;
            }
        }

        public async Task<int> GetTotalUsersCount()
        {
            return await _userRepository.GetTotalCountAsync();
        }

        public async Task<IEnumerable<User>> GetUsers(int page, int pageSize)
        {
            return await _userRepository.GetUsersAsync(page, pageSize);
        }

        Task<User> IUserService.GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        Task<User> IUserService.CreateUser(User user)
        {
            throw new NotImplementedException();
        }

        Task<User> IUserService.CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }
    }
}