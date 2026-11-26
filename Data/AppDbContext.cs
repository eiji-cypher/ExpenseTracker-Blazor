using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<SystemNotification> Notifications => Set<SystemNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(e =>
        {
            e.Property(t => t.Amount).HasPrecision(18, 2);
            e.Property(t => t.Description).HasMaxLength(200);

            e.Property(t => t.Type).HasConversion<string>().HasMaxLength(20);
            e.Property(t => t.Category).HasConversion<string>().HasMaxLength(20);

            e.HasIndex(t => t.Date);
            e.HasIndex(t => t.IsDeleted);
        });

        modelBuilder.Entity<SystemNotification>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.Type).HasConversion<string>().HasMaxLength(30);
            e.Property(n => n.Title).HasMaxLength(200);
            e.Property(n => n.Message).HasMaxLength(500);
            e.Property(n => n.LinkUrl).HasMaxLength(200);
            e.HasIndex(n => n.Timestamp);
        });
    }
}
