using GameShelf.API.Backend.Data;
using GameShelf.API.Backend.Models;
using GameShelf.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Security.Claims;
using static GameShelf.API.DTOs.LibraryDtos;

namespace GameShelf.API.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGamesService _gamesService;
        public LibraryService(ApplicationDbContext db, IHttpContextAccessor httpContext)
        {
            _db = db;
            _httpContextAccessor = httpContext;
        }

        /// <summary>
        /// Retrieves the user ID from the current HTTP context.
        /// </summary>
        /// <remarks>This method extracts the user ID from the claims of the currently authenticated user 
        /// in the HTTP context. If the HTTP context or the user is not available, or if the  claim for the user ID is
        /// not present, the method returns <see langword="null"/>.</remarks>
        /// <returns>The user ID as a string if available; otherwise, <see langword="null"/>.</returns>
        public  string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        /// <summary>
        /// Retrieves the list of games associated with the current user.
        /// </summary>
        /// <remarks>This method fetches all games from the database that are linked to the currently
        /// authenticated user. If the user is not authenticated or their user ID is unavailable, an empty list is
        /// returned.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="UserGame"/>
        /// objects representing the games in the user's library. If no games are found or the user is not
        /// authenticated, the list will be empty.</returns>
        //public async Task<List<UserGame>> GetUserLibraryAsync()
        //{
        //    var userIdInt = GetUserId();
        //    return await _db.UserGames
        //        .Include(ug => ug.Game) // Carica i dati del gioco correlato
        //        .Where(ug => ug.UserId == int.Parse(userIdInt))
        //        .ToListAsync();
        //}i


        /// <summary>
        /// Retrieves a list of games from the user's library based on the specified filters and sorting options.
        /// </summary>
        /// <remarks>If no filters are provided, all games in the user's library are returned. The method
        /// ensures that the results are filtered and sorted based on the provided parameters.</remarks>
        /// <param name="status">The status of the games to filter by. If specified, only games matching the given <see cref="GameStatus"/>
        /// will be included.</param>
        /// <param name="rating">The user rating to filter by. If specified, only games with the specified rating will be included.</param>
        /// <param name="price">The maximum price to filter by. If specified, only games with a price less than or equal to this value will
        /// be included.</param>
        /// <param name="platform">The platform to filter by. If specified, only games matching the given platform (case-insensitive) will be
        /// included.</param>
        /// <param name="sortBy">The sorting option for the results. Supported values are: <list type="bullet">
        /// <item><description><c>"rating_desc"</c>: Sorts by user rating in descending order.</description></item>
        /// <item><description><c>"name_asc"</c>: Sorts by game name in ascending order.</description></item>
        /// <item><description><c>"price_desc"</c>: Sorts by price in descending order.</description></item> </list> If
        /// not specified or invalid, the results are sorted by game name in ascending order by default.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="UserGame"/>
        /// objects that match the specified filters and sorting options.</returns>
        public async Task<List<UserGame>> GetUserLibraryAsync(GameStatus? status,int? rating,decimal? price,string? platform,string? sortBy)
        {
            var userIdInt = GetUserId();

            var query = _db.UserGames
                .Include(ug => ug.Game)
                .Where(ug => ug.UserId == int.Parse(userIdInt))
                .AsQueryable();

            //  filtri 
            if (status.HasValue)
            {
                query = query.Where(ug => ug.Status == status.Value);
            }

            if (rating.HasValue)
            {
                query = query.Where(ug => ug.UserRating == rating.Value);
            }

            if (price.HasValue)
            {
                //giochi che costano meno o uguale al prezzo specificato
                query = query.Where(ug => ug.Price <= price.Value);
            }

            if (!string.IsNullOrWhiteSpace(platform))
            {
                //  piattaforma (ignorando maiuscole/minuscole)
                query = query.Where(ug => ug.Platform != null && ug.Platform.ToLower() == platform.ToLower());
            }

            // ordinamento 
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "rating_desc":
                        query = query.OrderByDescending(ug => ug.UserRating);
                        break;
                    case "name_asc":
                        query = query.OrderBy(ug => ug.Game.Name);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(ug => ug.Price);
                        break;
                    default:
                        // Ordinamento di default se il parametro non è valido
                        query = query.OrderBy(ug => ug.Game.Name);
                        break;
                }
            }
            else
            {
                // Ordinamento di default se non viene specificato nessun 'sortBy'
                query = query.OrderBy(ug => ug.Game.Name);
            }

            return await query.ToListAsync();
        }


        /// <summary>
        /// Retrieves a game from the user's library based on the specified game ID.
        /// </summary>
        /// <remarks>This method checks the current user's library for a game matching the specified ID. 
        /// If the user is not authenticated or the game is not found, the method returns <see
        /// langword="null"/>.</remarks>
        /// <param name="gameId">The unique identifier of the game to retrieve.</param>
        /// <returns>A <see cref="UserGame"/> object representing the game in the user's library if found;  otherwise, <see
        /// langword="null"/>.</returns>
        public async Task<UserGame?> GetGameFromLibraryAsync(int gameId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return null;

            return await _db.UserGames.Where(ug => ug.UserId == int.Parse(userId) && ug.Game.RawgGameId == gameId).FirstOrDefaultAsync();
        }

       
        /// <summary>
        /// Adds a game to the user's library if it is not already present.
        /// </summary>
        /// <remarks>If the game does not exist in the database, it will be fetched from the RAWG service
        /// and added  to the database before being associated with the user. Throws an exception if the game details 
        /// cannot be retrieved from RAWG.</remarks>
        /// <param name="dto">An object containing the details of the game to add, including the game's RAWG ID,  the desired status, and
        /// the platform.</param>
        /// <returns>A <see cref="UserGame"/> object representing the added game if the operation is successful;  otherwise, <see
        /// langword="null"/> if the game is already in the user's library.</returns>
        /// <exception cref="Exception">Thrown if the game details cannot be retrieved from the RAWG service.</exception>
        public async Task<UserGame?> AddGameToLibraryAsync(AddGameToLibraryDto dto)
        {
            var userIdInt = GetUserId();

            // Controlla se l'utente ha già questo gioco
            var existingUserGame = await _db.UserGames
                .FirstOrDefaultAsync(ug => ug.User.Id == int.Parse(userIdInt) && ug.Game.RawgGameId == dto.RawgGameId);
            if (existingUserGame != null) return null; // Gioco già in libreria

            // Logica "Trova o Crea" per il gioco
            var game = await _db.Games.FirstOrDefaultAsync(g => g.RawgGameId == dto.RawgGameId);

            if (game == null)
            {
                // Se il gioco non è nel nostro DB, recupera i dettagli da RAWG
                var gameDetails = await _gamesService.GetGameDetailsAsync(dto.RawgGameId);
                if (gameDetails == null) throw new Exception("Impossibile trovare i dettagli del gioco su RAWG.");

                game = new Game
                {
                    RawgGameId = gameDetails.Id,
                    Name = gameDetails.Name,
                    Description = gameDetails.Description,
                    BackgroundImageUrl = gameDetails.BackgroundImageUrl,
                    MetacriticScore = gameDetails.MetacriticScore,
                    Released = gameDetails.Released
                };
                _db.Games.Add(game);
            }

            var userGame = new UserGame
            {
                UserId = int.Parse(userIdInt),
                Game = game,
                Status = dto.Status,
                Platform = dto.Platform
            };
            userGame.Status = GameStatus.Wishlist; //QUANDO UN GIOCO VIENE AGGIUNTO ALLA LIBRERIA LO SETTIAMO COME LISTA DEI DESIDERI (DA GIOCARE)
            _db.UserGames.Add(userGame);
            await _db.SaveChangesAsync();
            return userGame;
        }

    

        public async Task<bool> UpdateGameInLibraryAsync(int rawgGameId, UpdateGameDto updateDto)
        {
            var userId = GetUserId();

            var userGame = await _db.UserGames.FirstOrDefaultAsync(ug => ug.User.Id == int.Parse(userId) && ug.Game.RawgGameId == rawgGameId);
            if(userGame == null)
            {
                return false;
            }

     
            userGame.Status = updateDto.Status;
            userGame.UserRating = updateDto.UserRating;
            
            await _db.SaveChangesAsync();
            return true;

        }

        public async Task<bool> RemoveGameFromLibraryAsync(int rawgGameId)
        {
            var userId = GetUserId();

            var userGame = await _db.UserGames.FirstOrDefaultAsync(ug => ug.User.Id == int.Parse(userId) && ug.Game.RawgGameId == rawgGameId);

            if (userGame == null)
            {
                return false;
            }

            _db.UserGames.Remove(userGame);
            await _db.SaveChangesAsync();
            return true;
        }

    }
}
