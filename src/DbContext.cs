using Microsoft.EntityFrameworkCore;
using GearstashBot.Models;

namespace GearstashBot
{
    public class StashBotDbContext : DbContext  
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<QueueItem> Queue { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=config/gearstashbot.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QueueItem>()
                .HasOne(qi => qi.Author)
                .WithMany(a => a.QueueItems);

            modelBuilder.Entity<QueueItem>()
                .HasIndex(q => new {
                    q.MessageId,
                    q.SourceUrl
                });
        }
    }
}