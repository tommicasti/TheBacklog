using GameShelf.API.Backend.Models;

namespace GameShelf.API.Services
{
    public interface ILibraryService
    {
        Task<List<UserGame>> GetUserLibraryAsync();

        Task<UserGame?> GetGameFromLibraryAsync(int  gameId);
    }
}
