using System.ComponentModel.DataAnnotations;

namespace AliasGame.Domain.Models
{
    public class Expression : Entity
    {
        [Required] public string Text { get; set; }
    }
}