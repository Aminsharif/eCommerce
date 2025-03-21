using System;
using System.Threading.Tasks;
using eCommerce.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using eCommerce.Core.Models;

namespace eCommerce.Infrastructure.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;

        public PasswordResetService(UserManager<User> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> ValidatePasswordResetTokenAsync(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            return await _userManager.VerifyUserTokenAsync(user, 
                _userManager.Options.Tokens.PasswordResetTokenProvider, 
                "ResetPassword", 
                token);
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetLink)
        {
            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                var smtpServer = smtpSettings["Server"];
                var smtpPort = int.Parse(smtpSettings["Port"]);
                var smtpUsername = smtpSettings["Username"];
                var smtpPassword = smtpSettings["Password"];
                var senderEmail = smtpSettings["SenderEmail"];
                var senderName = smtpSettings["SenderName"];

                using (var client = new SmtpClient(smtpServer, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(senderEmail, senderName),
                        Subject = "Password Reset Request",
                        Body = $@"
                            <h2>Password Reset Request</h2>
                            <p>You have requested to reset your password. Please click the link below to reset your password:</p>
                            <p><a href='{resetLink}'>Reset Password</a></p>
                            <p>If you did not request this password reset, please ignore this email.</p>
                            <p>This link will expire in 24 hours.</p>
                            <p>Best regards,<br>Your eCommerce Team</p>",
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(email);

                    await client.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
} 