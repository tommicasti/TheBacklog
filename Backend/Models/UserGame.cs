using GameShelf.API.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameShelf.API.Backend.Models;

public enum GameStatus { Backlog, Playing, Completed, Wishlist }

public class UserGame
{
    public int Id { get; set; } 
   
    
    // Relazione con l'utente
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    // Relazione con il gioco
    public int GameId { get; set; }
    [ForeignKey(nameof(GameId))]
    public Game Game { get; set; } = null!;

    // Dati specifici dell'utente per questo gioco
    public GameStatus Status { get; set; }
    [Range(1, 5)]
    public int? UserRating { get; set; }
    public decimal? Price { get; set; }
    public string? Platform { get; set; }


}