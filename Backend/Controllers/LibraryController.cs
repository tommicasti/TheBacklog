
using GameShelf.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static GameShelf.API.DTOs.LibraryDtos;

namespace GameShelf.API.Controllers
{
    [ApiController]
    [Route("api/library")]
    [Authorize]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;
        public LibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        // GET /api/library : Ottiene tutti i giochi nella libreria dell'utente
        /// <summary>
        /// Retrieves all games in the current user's library.
        /// </summary>
        /// <remarks>This endpoint is accessible via the HTTP GET method at the route <c>/api/library</c>.
        /// Ensure the user is authenticated before calling this method, as it operates on the current user's
        /// context.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the list of games in the user's library. The response may include
        /// an empty list if the user has no games in their library.</returns>
        [HttpGet]
        public async Task<IActionResult> getUserLibrary()
        {

            var library = await _libraryService.GetUserLibraryAsync();
            if (library == null)
            {
                return NotFound();
            }
            return Ok(library);
        }


        // GET /api/library/{rawgGameId} : Ottiene  il giocho che corrisponde all id  nella libreria dell'utente
        /// <summary>
        /// Retrieves a game from the user's library based on the specified RAWG game ID.
        /// </summary>
        /// <remarks>This method queries the user's library for a game matching the provided RAWG game ID.
        /// If the game is found, it returns an HTTP 200 response with the game details. If the game is not found, it
        /// returns an HTTP 404 response.</remarks>
        /// <param name="rawgGameId">The unique identifier of the game in the RAWG database.</param>
        /// <returns>An <see cref="IActionResult"/> containing the game details if found; otherwise, a <see
        /// cref="NotFoundResult"/> if the game does not exist in the user's library.</returns>
        [HttpGet("{rawgGameId}")]
        public async Task<IActionResult> GetGameFromLibrary(int rawgGameId)
        {
            var userGame = await _libraryService.GetGameFromLibraryAsync(rawgGameId);
            if (userGame == null)
            {
                return NotFound();
            }
            return Ok(userGame);
        }

        [HttpPost]
        public async Task<IActionResult> AddGameToLibrary([FromBody] AddGameDto addGameDto)
        {
            return null;
        }


    }
}
