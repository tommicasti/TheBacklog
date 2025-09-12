using Microsoft.EntityFrameworkCore;
using GameShelf.API.Backend.Models;

namespace GameShelf.API.Backend.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserGame> UserGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserGame>()
            .HasKey(ug => new { ug.UserId, ug.RawgGameId });
    }
}