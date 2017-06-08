using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using QAirMonitor.Domain.Models;

namespace QAirMonitor.Persist.Context
{
    public class AppDataContext : DbContext
    {
        public DbSet<ReadingModel> Readings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = new SqliteConnection("Filename=AppData.db");
            optionsBuilder.UseSqlite(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReadingModel>()
                .ToTable("Readings")
                .HasKey(r => r.ReadingID);
        }
    }
}
