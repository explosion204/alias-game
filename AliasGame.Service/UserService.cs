using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AliasGame.Domain.Models;
using AliasGame.Service.Interfaces;
using AliasGame.Service.Models;
using Microsoft.IdentityModel.Tokens;

namespace AliasGame.Service
{
    public class UserService : IUserService
    {
        private readonly IDataManager _dataManager;
        private readonly string _secretKey;
        

        public UserService(IDataManager dataManager, string secretKey)
        {
            _dataManager = dataManager;
            _secretKey = secretKey;
        }

        public bool CreateUser(string nickname, string password)
        {
            if (_dataManager.UserRepository.GetAllEntities().FirstOrDefault(x => x.Nickname == nickname) != null)
            {
                return false;
            }

            var user = new User
            {
                Id = default,
                Nickname = nickname,
                PasswordHash = ComputeHash(password),
                RefreshToken = GenerateRefreshToken()
            };

            _dataManager.UserRepository.SaveEntity(user);
            return true;
        }

        public bool ChangePassword(string accessToken, string currentPassword, string newPassword)
        {
            var token = new JwtSecurityToken(accessToken);
            var userIdClaim = token.Claims.FirstOrDefault(x => x.Type == "unique_name");

            if (userIdClaim == null || DateTime.UtcNow >= token.ValidTo) return false;

            var user = _dataManager.UserRepository.GetEntity(userIdClaim.Value);

            if (user.PasswordHash != ComputeHash(currentPassword)) return false;
            
            user.PasswordHash = ComputeHash(newPassword);
            _dataManager.UserRepository.SaveEntity(user);

            return true;
        }

        public User GetUserInfo(string accessToken)
        {
            var token = new JwtSecurityToken(accessToken);
            var userIdClaim = token.Claims.FirstOrDefault(x => x.Type == "unique_name");

            if (userIdClaim == null || DateTime.UtcNow >= token.ValidTo) return null;

            var user = _dataManager.UserRepository.GetEntity(userIdClaim.Value);

            return user;
        }
        
        public AuthResponse Authenticate(string nickname, string password)
        {
            var passwordHash = ComputeHash(password);
            var user = _dataManager.UserRepository.GetAllEntities()
                .FirstOrDefault(x => x.Nickname == nickname && x.PasswordHash == passwordHash);

            if (user == null) return null;

            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            _dataManager.UserRepository.SaveEntity(user);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
        }

        public bool RevokeRefreshToken(string refreshToken)
        {
            var user = _dataManager.UserRepository.GetAllEntities().FirstOrDefault(
                x => x.RefreshToken.Token == refreshToken);

            if (user == null) return false;
            if (user.RefreshToken.IsExpired) return false;
            
            user.RefreshToken.Expires = DateTime.UtcNow;
            _dataManager.UserRepository.SaveEntity(user);

            return true;
        }

        public AuthResponse RefreshAccessToken(string refreshToken)
        {
            var user = _dataManager.UserRepository.GetAllEntities().FirstOrDefault(
                x => x.RefreshToken.Token == refreshToken);
            if (user == null) return null;

            if (user.RefreshToken.IsExpired) return null;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            _dataManager.UserRepository.SaveEntity(user);

            var accessToken = GenerateAccessToken(user);

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        private string ComputeHash(string text)
        {
            var sha1 = new SHA1CryptoServiceProvider();
            var data = Encoding.ASCII.GetBytes(text);
            var sha1data = sha1.ComputeHash(data);

            return Encoding.ASCII.GetString(sha1data);
        }

        private string GenerateAccessToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id), 
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDesc);

            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);

                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(1)
                };
            }
        }
    }
}