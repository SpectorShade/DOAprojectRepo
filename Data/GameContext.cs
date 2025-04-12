using _211426_FinalProjectDOA.Models;
using Microsoft.EntityFrameworkCore;

namespace _211426_FinalProjectDOA.Data
{
    public class GameContext:DbContext
    {

        public GameContext(DbContextOptions<GameContext> options) : base(options) { }

        public DbSet<Character> Characters { get; set; }
        public DbSet<Move> Moves { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Character>()
                .HasMany(c => c.Moves)
                .WithOne(m => m.Character)
                .HasForeignKey(m => m.CharacterId);
        }
    }
}
