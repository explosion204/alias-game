using System.ComponentModel.DataAnnotations;

namespace AliasGame.Domain.Models
{
    public class Session : Entity
    {
        [Required] public string FirstPlayerId { get; set; }
        public string SecondPlayerId { get; set; }
        public string ThirdPlayerId { get; set; }
        public string FourthPlayerId { get; set; }
    }
}