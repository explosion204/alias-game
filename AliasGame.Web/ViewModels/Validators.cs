using System.Collections.Generic;

namespace AliasGame.ViewModels
{
    public static class Validators
    {
        public static Dictionary<string, string> ValidateSignUpModel(SignUpViewModel model)
        {
            var validationErrors = new Dictionary<string, string>();

            if (model.Nickname != null && string.IsNullOrWhiteSpace(model.Nickname) && model.Nickname.Length > 12)
            {
                validationErrors.Add("nickname", "Nickname length must be under 12 characters");
            }
            

            if (model.Password != null && string.IsNullOrWhiteSpace(model.Password) && model.Password.Length >= 8)
            {
                validationErrors.Add("password", "Password must contain at least 8 characters");
            }

            if (model.ConfirmPassword != null && string.IsNullOrWhiteSpace(model.ConfirmPassword) &&
                model.ConfirmPassword.Length >= 8)
            {
                validationErrors.Add("confirmPassword", "Password must contain at least 8 characters");
            }

            if (model.Password != model.ConfirmPassword)
            {
                validationErrors.Add("confirmPassword", "Passwords do not match");
            }

            return validationErrors;
        }

        public static Dictionary<string, string> ValidateSignInModel(SignInViewModel model)
        {
            var validationErrors = new Dictionary<string, string>();

            if (model.Nickname != null && string.IsNullOrWhiteSpace(model.Nickname) && model.Nickname.Length > 12)
            {
                validationErrors.Add("nickname", "Nickname length must be under 12 characters");
            }
            
            if (model.Password != null && string.IsNullOrWhiteSpace(model.Password) && model.Password.Length >= 8)
            {
                validationErrors.Add("password", "Password must contain at least 8 characters");
            }

            return validationErrors;
        }

        public static Dictionary<string, string> ValidateChangePasswordModel(ChangePasswordViewModel model)
        {
            var validationErrors = new Dictionary<string, string>();
            
            if (model.NewPassword != null && string.IsNullOrWhiteSpace(model.NewPassword) && 
                model.NewPassword.Length >= 8)
            {
                validationErrors.Add("newPassword", "Password must contain at least 8 characters");
            }

            if (model.ConfirmNewPassword != null && string.IsNullOrWhiteSpace(model.ConfirmNewPassword) &&
                model.ConfirmNewPassword.Length >= 8)
            {
                validationErrors.Add("confirmNewPassword", "Password must contain at least 8 characters");
            }

            if (model.NewPassword != model.ConfirmNewPassword)
            {
                validationErrors.Add("confirmPassword", "Passwords do not match");
            }

            return validationErrors;
        }
    }
}