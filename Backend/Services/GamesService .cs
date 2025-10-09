using GameShelf.API.DTOs;
using System.Text.Json;
using static GameShelf.API.DTOs.GameDtos;

namespace GameShelf.API.Services;

public class GamesService : IGamesService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _apiKey;

    public GamesService(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _clientFactory = clientFactory;
        _apiKey = configuration["RAWG:ApiKey"];
    }

    public async Task<IEnumerable<GameSummaryDto>> SearchGamesAsync(string query)
    {
        var client = _clientFactory.CreateClient();
        var requestUrl = $"https://api.rawg.io/api/games?key={_apiKey}&search={query}";
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode) return Enumerable.Empty<GameSummaryDto>();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var rawgResponse = JsonSerializer.Deserialize<RawgSearchResponse>(jsonResponse, options);

        return rawgResponse?.Results.Select(g => new GameSummaryDto
        {
            Id = g.Id,
            Name = g.Name,
            BackgroundImageUrl = g.BackgroundImage,
            MetacriticScore = g.Metacritic,
            Released = g.Released
        }) ?? Enumerable.Empty<GameSummaryDto>();
    }

    public async Task<GameDetailDto?> GetGameDetailsAsync(int rawgGameId)
    {
        var client = _clientFactory.CreateClient();
        var requestUrl = $"https://api.rawg.io/api/games/{rawgGameId}?key={_apiKey}";
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var rawgGame = JsonSerializer.Deserialize<RawgGameDetailResponse>(jsonResponse, options);

        if (rawgGame == null) return null;

        return new GameDetailDto
        {
            Id = rawgGame.Id,
            Name = rawgGame.Name,
            Description = rawgGame.DescriptionRaw,
            BackgroundImageUrl = rawgGame.BackgroundImage,
            MetacriticScore = rawgGame.Metacritic,
            Released = rawgGame.Released,
            Platforms = rawgGame.Platforms.Select(p => p.Platform.Name).ToList(),
            Genres = rawgGame.Genres.Select(g => g.Name).ToList()
        };
    }


    public async Task<IEnumerable<GameSummaryDto>> GetPopularGamesAsync()
    {
        var client = _clientFactory.CreateClient();
        var lastYear = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
        var today = DateTime.Now.ToString("yyyy-MM-dd");

        //  'ordering=-metacritic' per ordinare per voto della critica
        var requestUrl = $"https://api.rawg.io/api/games?key={_apiKey}&dates={lastYear},{today}&ordering=-added";

        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<GameSummaryDto>();
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var rawgResponse = JsonSerializer.Deserialize<RawgSearchResponse>(jsonResponse, options);

        // Mappiamo il risultato nel nostro DTO pulito
        return rawgResponse?.Results.Select(g => new GameSummaryDto
        {
            Id = g.Id,
            Name = g.Name,
            BackgroundImageUrl = g.BackgroundImage,
            MetacriticScore = g.Metacritic,
            Released = g.Released
        }) ?? Enumerable.Empty<GameSummaryDto>();
    }


}

// Classi helper per deserializzare le risposte di RAWG
file class RawgSearchResponse { public List<RawgGameResult> Results { get; set; } = new(); }
file class RawgGameResult { public int Id { get; set; } public string Name { get; set; } = ""; public string BackgroundImage { get; set; } = ""; public int? Metacritic { get; set; } public string Released { get; set; } = ""; }
file class RawgGameDetailResponse { public int Id { get; set; } public string Name { get; set; } = ""; public string DescriptionRaw { get; set; } = ""; public string BackgroundImage { get; set; } = ""; public int? Metacritic { get; set; } public string Released { get; set; } = ""; public List<RawgPlatform> Platforms { get; set; } = new(); public List<RawgGenre> Genres { get; set; } = new(); }
file class RawgPlatform { public RawgPlatformInfo Platform { get; set; } = new(); }
file class RawgPlatformInfo { public string Name { get; set; } = ""; }
file class RawgGenre { public string Name { get; set; } = ""; }