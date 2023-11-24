using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Inventory.Models.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventotryProjectPractice.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<RegisterUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<RegisterUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<bool> RegisterUser(RegisterUser register)
        {
            var identityUser = new RegisterUser
            {
                UserName = register.UserName,
                Email = register.Email,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpiryTime = DateTime.Now.AddDays(30),
                Age = register.Age,
                PhoneNumber = register.PhoneNumber
            };

            var result = await _userManager.CreateAsync(identityUser, register.Password);
            if (result.Succeeded)
            {
                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);

                SendConfirmationEmail(register.Email, emailConfirmationToken);
            }
            return result.Succeeded;
        }

        public async Task<bool> Login(LoginUser user)
        {
            var identityUser = await _userManager.FindByEmailAsync(user.Email);
            if (identityUser == null)
            {
                return false;
            }

            return await _userManager.CheckPasswordAsync(identityUser, user.Password);
        }

        public string GenerateTokenString(LoginUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "Admin"),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));
            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var securityToken = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Set token expiration to 1 hour
                issuer: _config.GetSection("Jwt:Issuer").Value,
                audience: _config.GetSection("Jwt:Audience").Value,
                signingCredentials: signingCred
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<string> RefreshToken(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return null;
            }

            // Generate a new refresh token and update the user's refresh token in the database.
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            // Generate a new access token with the updated claims.
            var newAccessToken = GenerateTokenString(new LoginUser { Email = user.Email });

            return newAccessToken;
        }
        // AuthService.cs

        private void SendConfirmationEmail(string email, string emailConfirmationToken)
        {
            // Replace these with your email server details
            string smtpServer = "your-smtp-server.com";
            int smtpPort = 587;
            string smtpUsername = "your-smtp-username";
            string smtpPassword = "your-smtp-password";
            string senderEmail = "your-sender-email";

            var confirmationLink = $"https://your-website.com/confirm-email?email={email}&token={emailConfirmationToken}";

            using var client = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = "Email Confirmation",
                IsBodyHtml = true,
                Body = $"<a href='{confirmationLink}'>Click here to confirm your email</a>"
            };

            mailMessage.To.Add(email);

            client.Send(mailMessage);
        }

    }
}
