using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DbDataSource
{
    public class GameContext : DbContext
    {
        public const string DbName = "TestGameDb";

        public GameContext(DbContextOptions options) : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Hand>()
                .Property(e => e.Cards)
                .HasConversion<string>();
        }

        public DbSet<Game> Games { get; set; }
    }
}
