namespace AliasGame.ViewModels
{
    public class ChangePasswordViewModel
    {
        public string AccessToken { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}