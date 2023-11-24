using Inventory.Models.Models;

namespace InventotryProjectPractice.Services
{
    public interface IAuthService
    {
        string GenerateTokenString(LoginUser user);
        Task<bool> Login(LoginUser user);
        Task<bool> RegisterUser(RegisterUser register);
        Task<string> RefreshToken(string email);
        string GenerateRefreshToken();

    }
}