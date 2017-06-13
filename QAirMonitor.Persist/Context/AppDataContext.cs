using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using QAirMonitor.Domain.Models;

namespace QAirMonitor.Persist.Context
{
    public class AppDataContext : DbContext
    {
        public DbSet<ReadingModel> Readings { get; set; }
        public DbSet<AuditLogModel> AuditLogEntries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = new SqliteConnection("Filename=AppData.db");
            optionsBuilder.UseSqlite(connection)
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ReadingModel>()
                .ToTable("Readings")
                .HasKey(r => r.ReadingID);

            modelBuilder.Entity<AuditLogModel>()
                .ToTable("AuditLog")
                .HasKey(l => l.AuditLogID);

            modelBuilder.Entity<AuditLogModel>()
                .Property(l => l.Message)
                .HasMaxLength(2000);
        }
    }
}
