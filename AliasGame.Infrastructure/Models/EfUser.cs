using System;

namespace AliasGame.Infrastructure.Models
{
    public class EfUser
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string PasswordHash { get; set; }
        public int TotalGames { get; set; }
        public int Wins { get; set; }
        
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
    }
}