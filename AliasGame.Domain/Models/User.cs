using System.ComponentModel.DataAnnotations;

namespace AliasGame.Domain.Models
{
    public class User : Entity
    {
        [Required] public string Nickname { get; set; }
        [Required] public string PasswordHash { get; set; }

        [Required] public int TotalGames { get; set; }
        [Required] public int Wins { get; set; }
        
        public RefreshToken RefreshToken { get; set; }
    }
}