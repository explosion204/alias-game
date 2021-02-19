using System.ComponentModel.DataAnnotations;

namespace AliasGame.Domain.Models
{
    public class User : Entity
    {
        [Required] public string Email { get; set; }
        [Required] public bool EmailConfirmed { get; set; }
        [Required] public string PasswordHash { get; set; }
        public string Username { get; set; }
        [Required] public int TotalGames { get; set; }
        [Required] public int Wins { get; set; }
    }
}