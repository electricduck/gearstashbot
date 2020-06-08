using Microsoft.EntityFrameworkCore;
using StashBot.Models;

namespace StashBot
{
    public class StashBotDbContext : DbContext  
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<QueueItem> Queue { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=stashbot.db");

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