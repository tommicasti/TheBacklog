using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShelf.API.Backend.Models;

// Definiamo un enum per lo stato del gioco
public enum GameStatus
{
    Backlog,
    Playing,
    Completed,
    Wishlist
}

public class UserGame
{
    public int Id { get; set; } 

    [Required]
    public int UserId { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    [Required]
    public int RawgGameId { get; set; } // ID del gioco su RAWG

    public string GameTitle { get; set; } = string.Empty;
    public GameStatus Status { get; set; }

 
}