using GameShelf.API.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using static GameShelf.API.DTOs.GameDtos;

namespace GameShelf.API.Services;

public class GamesService : IGamesService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly string _apiKey;
    private readonly IMemoryCache _cache;

    private static CancellationTokenSource _resetCacheToken = new();// per invalidare la cache

    public GamesService(IHttpClientFactory clientFactory, IConfiguration configuration, IMemoryCache cache)
    {
        _clientFactory = clientFactory;
        _apiKey = configuration["RAWG:ApiKey"];
        _cache= cache;
    }

    public async Task<IEnumerable<GameSummaryDto>> SearchGamesAsync(string query)
    {
        var cacheKey = $"search-{query.ToLower()}";
        // Cerchiamo i dati nella cache. 
        if (_cache.TryGetValue(cacheKey, out IEnumerable<GameSummaryDto>? cachedGames))
        {
           //  i dati sono in cache, li restituiamo immediatamente!
            return cachedGames ?? Enumerable.Empty<GameSummaryDto>();
        }
        // Se i dati non sono in cache, eseguiamo la logica esistente per chiamare RAWG
        var client = _clientFactory.CreateClient();
        var requestUrl = $"https://api.rawg.io/api/games?key={_apiKey}&search={query}";
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode) return Enumerable.Empty<GameSummaryDto>();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var rawgResponse = JsonSerializer.Deserialize<RawgSearchResponse>(jsonResponse, options);

        var games= rawgResponse?.Results.Select(g => new GameSummaryDto
        {
            Id = g.Id,
            Name = g.Name,
            BackgroundImageUrl = g.BackgroundImage,
            MetacriticScore = g.Metacritic,
            Released = g.Released
        }) ?? Enumerable.Empty<GameSummaryDto>();
        
        // Prima di restituire i dati, li salviamo nella cache per le prossime richieste
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(1)) // Scadenza cache (es. 1 ora)
            .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token)); // Collega all'interruttore

        _cache.Set(cacheKey, games, cacheOptions);
        return games;

    }

    public async Task<GameDetailDto?> GetGameDetailsAsync(int rawgGameId)
    {
        var cacheKey = $"game-{rawgGameId}";
        // Controllo della cache
        if (_cache.TryGetValue(cacheKey, out GameDetailDto? cachedGame))
        {
            return cachedGame;
        }

        // Chiamata all'API se non è in cache
        var client = _clientFactory.CreateClient();
        var requestUrl = $"https://api.rawg.io/api/games/{rawgGameId}?key={_apiKey}";
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode) return null;

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var rawgGame = JsonSerializer.Deserialize<RawgGameDetailResponse>(jsonResponse, options);

        if (rawgGame == null) return null;

        var gameDetails = new GameDetailDto
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
        
        // 4. Salvataggio in cache
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromDays(1)) // I dettagli di un gioco cambiano raramente, possiamo tenerli per più tempo
            .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

        _cache.Set(cacheKey, gameDetails, cacheOptions);

    }


    public async Task<IEnumerable<GameSummaryDto>> GetPopularGamesAsync()
    {
        //  Chiave statica per questa chiamata
        const string cacheKey = "popular-games";

        // Controllo della cache
        if (_cache.TryGetValue(cacheKey, out IEnumerable<GameSummaryDto>? cachedGames))
        {
            return cachedGames ?? Enumerable.Empty<GameSummaryDto>();
        }

        //  Chiamata all'API se non è in cache
        var client = _clientFactory.CreateClient();
        var lastYear = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
        var today = DateTime.Now.ToString("yyyy-MM-dd");

        //  'ordering=-metacritic' per ordinare per voto della critica
        var requestUrl = $"https://api.rawg.io/api/games?key={_apiKey}&dates={lastYear},{today}&ordering=-metacritic";

        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            return Enumerable.Empty<GameSummaryDto>();
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var rawgResponse = JsonSerializer.Deserialize<RawgSearchResponse>(jsonResponse, options);

        // Mappiamo il risultato nel nostro DTO pulito
        var popularGames = rawgResponse?.Results.Select(g => new GameSummaryDto
        {
            Id = g.Id,
            Name = g.Name,
            BackgroundImageUrl = g.BackgroundImage,
            MetacriticScore = g.Metacritic,
            Released = g.Released
        }) ?? Enumerable.Empty<GameSummaryDto>();

        //Salvataggio in cache
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(6)) // La lista dei giochi popolari può essere aggiornata meno di frequente di una ricerca
            .AddExpirationToken(new CancellationChangeToken(_resetCacheToken.Token));

        _cache.Set(cacheKey, popularGames, cacheOptions);

        return popularGames;
    }

    public void ClearCache()
    {
        // Se l'interruttore esiste, lo "premiamo"
        if (_resetCacheToken != null && !_resetCacheToken.IsCancellationRequested && _resetCacheToken.Token.CanBeCanceled)
        {
            _resetCacheToken.Cancel(); // Invalida tutte le voci collegate
            _resetCacheToken.Dispose();
        }

        // Creiamo un nuovo interruttore per le future voci di cache
        _resetCacheToken = new CancellationTokenSource();
    }
}

// Classi helper per deserializzare le risposte di RAWG
file class RawgSearchResponse { public List<RawgGameResult> Results { get; set; } = new(); }
file class RawgGameResult { public int Id { get; set; } public string Name { get; set; } = ""; public string BackgroundImage { get; set; } = ""; public int? Metacritic { get; set; } public string Released { get; set; } = ""; }
file class RawgGameDetailResponse { public int Id { get; set; } public string Name { get; set; } = ""; public string DescriptionRaw { get; set; } = ""; public string BackgroundImage { get; set; } = ""; public int? Metacritic { get; set; } public string Released { get; set; } = ""; public List<RawgPlatform> Platforms { get; set; } = new(); public List<RawgGenre> Genres { get; set; } = new(); }
file class RawgPlatform { public RawgPlatformInfo Platform { get; set; } = new(); }
file class RawgPlatformInfo { public string Name { get; set; } = ""; }
file class RawgGenre { public string Name { get; set; } = ""; }