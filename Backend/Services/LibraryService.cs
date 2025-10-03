using GameShelf.API.Backend.Data;
using GameShelf.API.Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameShelf.API.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

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
        public async Task<List<UserGame>> GetUserLibraryAsync()
        {
            var userId  = GetUserId();
            if (string.IsNullOrEmpty(userId)) return new List<UserGame>();

            
            return await _db.UserGames.Where(ug => ug.UserId  == int.Parse(userId)).ToListAsync();
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

            return await _db.UserGames.Where(ug => ug.UserId == int.Parse(userId) && ug.RawgGameId == gameId).FirstOrDefaultAsync();
        }


     }
}
