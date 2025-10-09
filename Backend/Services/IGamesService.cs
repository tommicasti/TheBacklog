using GameShelf.API.DTOs;
using static GameShelf.API.DTOs.GameDtos;

namespace GameShelf.API.Services
{
    public interface IGamesService
    {
        Task<IEnumerable<GameSummaryDto>> SearchGamesAsync(string query);
        Task<GameDetailDto?> GetGameDetailsAsync(int rawgGameId);
        Task<IEnumerable<GameSummaryDto>> GetPopularGamesAsync(); 
    }
}
