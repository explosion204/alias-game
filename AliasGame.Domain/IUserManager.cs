using System;
using System.Threading.Tasks;
using AliasGame.Domain.Models;

namespace AliasGame.Domain
{
    public interface IUserManager
    {
        Task<Guid> CreateUser(User newUser, string password);
        Task DeleteUser(Guid id);
        
        Task<User> FindUserById(Guid id);
        Task<User> FindUserByEmail(string email);

        Task ChangeUsername(Guid userId, string nickname);
        Task<bool> ChangeUserPassword(Guid userId, string currentPassword, string newPassword);
        
        Task<bool> SignIn(string username, string password);
        Task SignOut();
        
        Task<bool> ConfirmEmail(Guid userId, string token);
        Task<bool> ResetPassword(Guid userId, string token, string newPassword);
        Task<string> GenerateEmailConfirmationToken(Guid userId);
        Task<string> GeneratePasswordResetToken(Guid userId);
    }
}