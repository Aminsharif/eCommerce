using eCommerce.Core.DTOs.Auth;
using eCommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eCommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IPasswordResetService _passwordResetService;

        public AuthController(IAuthService authService, IPasswordResetService passwordResetService)
        {
            _authService = authService;
            _passwordResetService = passwordResetService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            var response = await _authService.LoginAsync(loginDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            var response = await _authService.RegisterAsync(registerDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] string refreshToken)
        {
            var response = await _authService.RefreshTokenAsync(refreshToken);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<ActionResult<bool>> RevokeToken([FromBody] string refreshToken)
        {
            var result = await _authService.RevokeTokenAsync(refreshToken);
            if (!result)
            {
                return BadRequest("Invalid refresh token");
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("validate-token")]
        public async Task<ActionResult<bool>> ValidateToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(isValid);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _passwordResetService.GeneratePasswordResetTokenAsync(forgotPasswordDto.Email);
            if (token == null)
                return Ok(); // Don't reveal if the email exists

            var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?email={Uri.EscapeDataString(forgotPasswordDto.Email)}&token={Uri.EscapeDataString(token)}";
            
            var emailSent = await _passwordResetService.SendPasswordResetEmailAsync(forgotPasswordDto.Email, resetLink);
            if (!emailSent)
                return StatusCode(500, "Failed to send password reset email");

            return Ok();
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var isValidToken = await _passwordResetService.ValidatePasswordResetTokenAsync(
                resetPasswordDto.Email, 
                resetPasswordDto.Token);

            if (!isValidToken)
                return BadRequest("Invalid or expired password reset token");

            var success = await _passwordResetService.ResetPasswordAsync(
                resetPasswordDto.Email,
                resetPasswordDto.Token,
                resetPasswordDto.NewPassword);

            if (!success)
                return StatusCode(500, "Failed to reset password");

            return Ok();
        }
    }
} 