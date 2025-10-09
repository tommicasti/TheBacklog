using GameShelf.API.Backend.Models;
using GameShelf.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameShelf.API.Backend.Data;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; } 
    public DbSet<UserGame> UserGames { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Imposta RawgGameId come univoco per evitare duplicati di giochi
        modelBuilder.Entity<Game>()
            .HasIndex(g => g.RawgGameId)
            .IsUnique();

        // Imposta la coppia UserId-GameId come univoca per evitare che un utente
        // aggiunga lo stesso gioco più volte alla sua libreria
        modelBuilder.Entity<UserGame>()
            .HasIndex(ug => new { ug.UserId, ug.GameId })
            .IsUnique();
    }
}