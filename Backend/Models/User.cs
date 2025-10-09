namespace GameShelf.API.Backend.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // un utente ha una collezione di giochi nella sua libreria
    public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();
}