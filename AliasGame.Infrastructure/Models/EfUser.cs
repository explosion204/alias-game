using System;
using Microsoft.AspNetCore.Identity;

namespace AliasGame.Infrastructure.Models
{
    public class EfUser : IdentityUser<Guid>
    {
        public int TotalGames { get; set; }
        public int Wins { get; set; }
    }
}