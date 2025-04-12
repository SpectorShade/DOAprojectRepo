using System.ComponentModel.DataAnnotations;

namespace _211426_FinalProjectDOA.Models
{
    public class Move
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the move
        public string Name { get; set; } // Name of the move (e.g., "Jab", "Main Cannon")
        public string Input { get; set; } // Input notation (e.g., "6P", "P+K")
        public int Startup { get; set; } // Start-up frames
        public int Active { get; set; } // Active frames
        public int Recovery { get; set; } // Recovery frames
        public int TotalFrames => Startup + Active + Recovery; // Total frames (calculated property)
        public int AccumulatedStartup { get; set; } // Accumulated start-up for strings
        public int BlockAdvantage { get; set; } // Advantage left after block
        public int CharacterId { get; set; } // Foreign key reference to Character
        public Character Character { get; set; } // Navigation property
    }
}
