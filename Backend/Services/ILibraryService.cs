using GameShelf.API.Backend.Models;
using static GameShelf.API.DTOs.LibraryDtos;

namespace GameShelf.API.Services
{
    public interface ILibraryService
    {
        //Task<List<UserGame>> GetUserLibraryAsync();
        Task<List<UserGame>> GetUserLibraryAsync(
              GameStatus? status,
              int? rating,
              decimal? price,
              string? platform,
              string? sortBy
          );
        Task<UserGame?> GetGameFromLibraryAsync(int rawgGameId);
        Task<UserGame?> AddGameToLibraryAsync(AddGameToLibraryDto gameDto);
        Task<bool> UpdateGameInLibraryAsync(int rawgGameId, UpdateGameDto updateDto);
        Task<bool> RemoveGameFromLibraryAsync(int rawgGameId);
    }
}
