using AliasGame.Domain.Models;
using AliasGame.Service.Models;

namespace AliasGame.Service.Interfaces
{
    public interface IUserService
    {
        bool CreateUser(string nickname, string password);
        bool ChangePassword(string accessToken, string currentPassword, string newPassword);
        User GetUserInfo(string accessToken);
        AuthResponse Authenticate(string nickname, string password);
        AuthResponse RefreshAccessToken(string token);
        public bool RevokeRefreshToken(string refreshToken);
    }
}