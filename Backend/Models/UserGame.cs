using System.ComponentModel.DataAnnotations;

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
    public string UserId { get; set; } = string.Empty;

    [Required]
    public int RawgGameId { get; set; } // ID del gioco su RAWG

    public string GameTitle { get; set; } = string.Empty;
    public GameStatus Status { get; set; }

 
}