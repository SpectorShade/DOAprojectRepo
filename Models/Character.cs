using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace _211426_FinalProjectDOA.Models
{
    public class Character
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the character
        public string Picture { get; set; } // URL or file path to the character's picture
        public string Name { get; set; } // Character's name
        public List<Move> Moves { get; set; } // List of the character's moves
    }
}
