using Azure.Core;
using Inventory.Models.Models;
using InventotryProjectPractice.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventotryProjectPractice.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<RegisterUser> _userManager;
        public AuthController(IAuthService authService, UserManager<RegisterUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser(RegisterUser register)
        {
            if (await _authService.RegisterUser(register))
            {
/*                return Created("api/Auth/Register", new { message = "Successfully done" });
*/                return Ok(new { message = "Successfully done" });

            }
            return BadRequest("something went wrong");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (await _authService.Login(user))
            {
                var tokenString = _authService.GenerateTokenString(user);
                // Set the refresh token in a cookie
                var refreshCookieValue = _authService.GenerateRefreshToken();
                Response.Cookies.Append("refreshToken", refreshCookieValue, new CookieOptions
                {
                    HttpOnly = true, // Prevent client-side JavaScript access
                    Secure = true,    // Requires HTTPS
                    Expires = DateTime.UtcNow.AddMonths(1), // Set an appropriate expiration time
                    SameSite = SameSiteMode.None
                });

                return Ok(new { AccessToken = tokenString});
            }
            return BadRequest();
        }
        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RegisterUser model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.RefreshToken))
            {
                return BadRequest("Invalid client request");
            }

            string newAccessToken = await _authService.RefreshToken(model.Email);

            if (newAccessToken != null)
            {
                return Ok(new { accessToken = newAccessToken });
            }
            else
            {
                return BadRequest("Invalid user or refresh token");
            }
        }
        [HttpGet]
        [Route("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                // Handle missing email or token
                return BadRequest("Email and token are required.");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Handle user not found
                return NotFound("User not found.");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Successful email confirmation
                return Ok("Email confirmed successfully.");
            }
            else
            {
                // Handle email confirmation failure
                return BadRequest("Email confirmation failed.");
            }
        }


    }
}
