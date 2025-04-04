using Microsoft.AspNetCore.Mvc;
using eCommerce.Core.Interfaces;
using eCommerce.Core.Models;
using eCommerce.Core.Enums;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace eCommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmail(email);
                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user with email {email}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(UserRole role)
        {
            try
            {
                var users = await _userService.GetUsersByRole(role);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving users with role {role}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            user.UserName = user.Email.Split('@')[0]; // Generate username from email
            user.Role = UserRole.Customer;
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;

            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _userService.AuthenticateUser(request.Email, request.Password);
                if (user == null)
                    return Unauthorized();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                await _userService.DeactivateUser(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for user {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("{id}/addresses")]
        public async Task<IActionResult> AddAddress(int id, [FromBody] Address address)
        {
            try
            {
                await _userService.AddAddress(id, address);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding address for user {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{userId}/addresses/{addressId}")]
        public async Task<IActionResult> UpdateAddress(int userId, int addressId, [FromBody] Address address)
        {
            try
            {
                if (addressId != address.Id)
                    return BadRequest();

                await _userService.UpdateAddress(userId, address);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating address {addressId} for user {userId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId}/addresses/{addressId}")]
        public async Task<IActionResult> DeleteAddress(int userId, int addressId)
        {
            try
            {
                await _userService.DeleteAddress(userId, addressId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting address {addressId} for user {userId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/orders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetUserOrders(int id)
        {
            try
            {
                var orders = await _userService.GetUserOrders(id);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving orders for user {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}/reviews")]
        public async Task<ActionResult<IEnumerable<Review>>> GetUserReviews(int id)
        {
            try
            {
                var reviews = await _userService.GetUserReviews(id);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving reviews for user {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] string email)
        {
            try
            {
                var success = await _userService.ResetPassword(email);
                if (!success)
                    return BadRequest("Failed to reset password");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting password for user with email {email}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var success = await _userService.ChangePassword(request.UserId, request.CurrentPassword, request.NewPassword);
                if (!success)
                    return BadRequest("Failed to change password");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing password for user {request.UserId}");
                return StatusCode(500, "Internal server error");
            }
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        public class ChangePasswordRequest
        {
            public int UserId { get; set; }
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
        }
    }
} 