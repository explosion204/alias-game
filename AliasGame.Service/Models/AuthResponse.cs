using System.Text.Json.Serialization;

namespace AliasGame.Service.Models
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }

        [JsonIgnore] public string RefreshToken { get; set; }
    }
}