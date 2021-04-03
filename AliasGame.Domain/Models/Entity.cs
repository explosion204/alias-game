using System;
using System.ComponentModel.DataAnnotations;

namespace AliasGame.Domain.Models
{
    public class Entity
    {
        [Required] public string Id { get; set; }
    }
}