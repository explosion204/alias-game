using System;

namespace AliasGame.Infrastructure.Models
{
    public class EfSession
    {
        public Guid Id { get; set; }
        public string FirstPlayerId { get; set; }
        public string SecondPlayerId { get; set; }
        public string ThirdPlayerId { get; set; }
        public string FourthPlayerId { get; set; }
    }
}