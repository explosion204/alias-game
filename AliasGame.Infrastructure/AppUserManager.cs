using System;
using System.Threading.Tasks;
using AliasGame.Domain;
using AliasGame.Domain.Models;
using AliasGame.Infrastructure.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AliasGame.Infrastructure
{
    public class AppUserManager : IUserManager
    {
        private readonly UserManager<EfUser> _userManager;
        private readonly SignInManager<EfUser> _signInManager;
        private readonly IMapper _mapper;

        public AppUserManager(UserManager<EfUser> userManager, SignInManager<EfUser> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }
        
        public async Task<Guid> CreateUser(User newUser, string password)
        {
            var efUser = new EfUser()
            {
                Id = newUser.Id,
                UserName = newUser.Username,
                Email = newUser.Email,
                EmailConfirmed = newUser.EmailConfirmed,
                TotalGames = newUser.TotalGames,
                Wins = newUser.Wins
            };

            var userCreation = await _userManager.CreateAsync(efUser, password);
            return userCreation.Succeeded ? efUser.Id : default;
        }

        public async Task DeleteUser(Guid id)
        {
            var efUser = await _userManager.FindByIdAsync(id.ToString());

            if (efUser != null)
            {
                await _userManager.DeleteAsync(efUser);
            }
        }

        public async Task<User> FindUserById(Guid id)
        {
            var efUser = await _userManager.FindByIdAsync(id.ToString());
            return _mapper.Map<User>(efUser);
        }

        public async Task<User> FindUserByEmail(string email)
        {
            var efUser = await _userManager.FindByEmailAsync(email);
            return efUser != null ? _mapper.Map<User>(efUser) : null;
        }

        public async Task ChangeUsername(Guid userId, string nickname)
        {
            var efUser = await _userManager.FindByIdAsync(userId.ToString());
            efUser.UserName = nickname;
            await _userManager.UpdateAsync(efUser);
        }

        public async Task<bool> ChangeUserPassword(Guid userId, string currentPassword, string newPassword)
        {
            var efUser = await _userManager.FindByIdAsync(userId.ToString());
            var result = await _userManager.ChangePasswordAsync(efUser, currentPassword, newPassword);
            return result.Succeeded;
        }

        public async Task<bool> SignIn(string username, string password)
        {
            var efUser = await _userManager.FindByNameAsync(username);
            
            if (efUser != null)
            {
                await _signInManager.SignOutAsync();
                var result =
                    await _signInManager.PasswordSignInAsync(efUser, password, false, false);

                return result.Succeeded;
            }

            return false;
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> ConfirmEmail(Guid userId, string token)
        {
            var efUser = await _userManager.FindByIdAsync(userId.ToString());

            if (efUser != null)
            {
                var status = await _userManager.ConfirmEmailAsync(efUser, token);
                return status.Succeeded;
            }

            return false;
        }

        public async Task<bool> ResetPassword(Guid userId, string token, string newPassword)
        {
            var efUser = await _userManager.FindByIdAsync(userId.ToString());

            if (efUser != null)
            {
                var status = await _userManager.ResetPasswordAsync(efUser, token, newPassword);
                return status.Succeeded;
            }

            return false;
        }

        public async Task<string> GenerateEmailConfirmationToken(Guid userId)
        {
            var efUser = await _userManager.FindByIdAsync(userId.ToString());
            return await _userManager.GenerateEmailConfirmationTokenAsync(efUser);
        }

        public async Task<string> GeneratePasswordResetToken(Guid userId)
        {
            var efUser = await _userManager.FindByIdAsync(userId.ToString());
            return await _userManager.GeneratePasswordResetTokenAsync(efUser);
        }
    }
}