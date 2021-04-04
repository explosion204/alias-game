using System;
using AliasGame.Service.Interfaces;
using AliasGame.Service.Models;
using AliasGame.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AliasGame.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        /*
         * {
         *     "status": true/false,
         *     "userId": "user id",
         *     "nickname": "user nickname",
         *     "totalGames": "user total games",
         *     "wins": "user wins"
         * }
         * false status means that access token is invalid or expired,
         * use refresh token to obtain new access key
         */
        [HttpGet("is_authenticated/{accessToken}")]
        public IActionResult IsAuthenticated(string accessToken)
        {
            var user = _userService.GetUserFromToken(accessToken);
            var opStatus = user != null;

            return Ok(new
            {
                status = opStatus,
                userId = user?.Id,
                nickname = user?.Nickname,
                totalGames = user?.TotalGames,
                wins = user?.Wins
            });
        }

        [HttpGet("sign_out")]
        public new IActionResult SignOut()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                _userService.RevokeRefreshToken(refreshToken);
            }

            return Ok();
        }

        /*
         * {
         *     "status": true/false,
         *     "validationErrors":
         *     {
         *         "fieldName": "error desc"
         *     }
         * }
         */
        [HttpPost("sign_up")]
        public IActionResult SignUp([FromBody] SignUpViewModel viewModel)
        {
            var errors = Validators.ValidateSignUpModel(viewModel);
            var opStatus = false;

            if (errors.Count == 0)
            {
                opStatus = _userService.CreateUser(viewModel.Nickname, viewModel.Password);
            }

            /*
             * false && errors.Count > 0 => validation errors
             * false && errors.Count == 0 => duplicate email
             * true && errors.Count == 0 => successful sign up
             */
            return Ok(new
            {
                status = opStatus,
                validationErrors = errors
            });
        }

        /*
         * {
         *     "status": true/false,
         *     "validationErrors":
         *     {
         *         "fieldName": "error desc"
         *     },
         *     "accessToken": "access token"
         *    
         * }
         */
        [HttpPost("sign_in")]
        public IActionResult SignIn([FromBody] SignInViewModel viewModel)
        {
            var errors = Validators.ValidateSignInModel(viewModel);
            var opStatus = false;
            AuthResponse response = null;

            if (errors.Count == 0)
            {
                response = _userService.Authenticate(viewModel.Nickname, viewModel.Password);
                opStatus = response != null;

                if (opStatus)
                {
                    SetRefreshToken(response.RefreshToken);
                }
            }

            /*
             * false && errors.Count > 0 => validation errors
             * false && errors.Count == 0 => incorrect email or password
             * true && errors.Count == 0 => successful sign in
             */
            return Ok(new
            {
                status = opStatus,
                validationErrors = errors,
                accessToken = response?.AccessToken
            });
        }

        /*
         * {
         *     "status": true/false,
         *     "validationErrors":
         *     {
         *         "fieldName": "error desc"
         *     }
         * }
         * false status means that access token is expired or current password is wrong
         */
        [HttpPost("change_password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordViewModel viewModel)
        {
            var errors = Validators.ValidateChangePasswordModel(viewModel);
            var opStatus = false;

            if (errors.Count == 0)
            {
                opStatus = _userService.ChangePassword(viewModel.AccessToken, viewModel.CurrentPassword, viewModel.NewPassword);
            }

            return Ok(new
            {
                status = opStatus,
                validationErrors = errors
            });
        }

        /*
         * {
         *     "status": true/false,
         *     "accessToken": "new access token"
         * }
         * false status means refresh token is invalid or expired
         */
        [HttpPost("refresh_token")]
        public IActionResult RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = _userService.RefreshAccessToken(refreshToken);
            var opStatus = false;

            if (response != null)
            {
                opStatus = true;
                SetRefreshToken(response.RefreshToken);
            }

            return Ok(new
            {
                status = opStatus,
                accessToken = response?.AccessToken
            });
        }

        private void SetRefreshToken(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(1)
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}