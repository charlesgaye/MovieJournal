using Microsoft.EntityFrameworkCore;
using MovieJournal.Models;

namespace MovieJournal.Data;

public class AppDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>()
            .HasIndex(m => new { m.Title, m.Director, m.Year })
            .IsUnique();
    }
}