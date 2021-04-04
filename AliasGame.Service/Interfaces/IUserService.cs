using AliasGame.Domain.Models;
using AliasGame.Service.Models;

namespace AliasGame.Service.Interfaces
{
    public interface IUserService
    {
        bool CreateUser(string nickname, string password);
        bool ChangePassword(string accessToken, string currentPassword, string newPassword);
        User GetUserFromToken(string accessToken); // get user from access token
        User GetUserById(string userId); // get user by id
        User GetUserById(string accessToken, string userId); // get user by id if access token is not expired
        AuthResponse Authenticate(string nickname, string password);
        AuthResponse RefreshAccessToken(string token);
        public bool RevokeRefreshToken(string refreshToken);
    }
}