
using GameShelf.API.Backend.Models;
using GameShelf.API.DTOs;
using GameShelf.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;
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
        //[HttpGet]
        //public async Task<IActionResult> GetUserLibrary()
        //{

        //    var library = await _libraryService.GetUserLibraryAsync();
        //    if (library == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(library);
        //}

        //Filtrare per giochi su PC nel Backlog:
        //GET api/library?platform=PC&status=Backlog
        //Filtrare per giochi che hai pagato meno di 20 euro:
        //GET api/library?price=19.99
        //Filtrare per giochi su PlayStation 5, con voto 5, ordinati per prezzo(dal più caro):
        //GET api/library?platform=PlayStation 5&rating=5&sortBy=price_desc
        /// <summary>
        /// Retrieves the user's game library based on the specified filters and sorting options.
        /// </summary>
        /// <remarks>This method allows users to retrieve their game library with optional filters for
        /// game status, rating, price, and platform. Results can also be sorted based on the specified field. If no
        /// filters or sorting options are provided, all games in the library are returned in the default
        /// order.</remarks>
        /// <param name="status">The status of the games to filter by. Can be null to include all statuses.</param>
        /// <param name="rating">The minimum rating of the games to include. Can be null to include all ratings.</param>
        /// <param name="price">The maximum price of the games to include. Can be null to include all price ranges.</param>
        /// <param name="platform">The platform of the games to filter by. Can be null to include all platforms.</param>
        /// <param name="sortBy">The field by which to sort the results. Can be null to use the default sorting.</param>
        /// <returns>An <see cref="IActionResult"/> containing the filtered and sorted list of games in the user's library.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUserLibrary([FromQuery] GameStatus? status,[FromQuery] int? rating,[FromQuery] decimal? price,[FromQuery] string? platform,[FromQuery] string? sortBy)
        {
            var userGames = await _libraryService.GetUserLibraryAsync( status, rating, price, platform, sortBy);
            return Ok(userGames);
        }



        // GET /api/library/{rawgGameId} : Ottiene  il gioco che corrisponde all id  nella libreria dell'utente
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

        /// <summary>
        /// Adds a game to the user's library based on the provided game details.
        /// </summary>
        /// <remarks>This method is intended to be used as an HTTP POST endpoint. Ensure that the provided
        /// <paramref name="addGameDto"/> contains valid data to avoid validation errors.</remarks>
        /// <param name="addGameDto">An object containing the details of the game to be added. This must include all required information for
        /// identifying and adding the game to the library.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Typically, this may include a success
        /// status or an error response if the operation fails.</returns>
        [HttpPost]
        public async Task<IActionResult> AddGameToLibrary([FromBody] AddGameToLibraryDto gameDto)
        {
            var result = await _libraryService.AddGameToLibraryAsync(gameDto);
            if (result == null)
            {
                return BadRequest("Gioco già presente nella libreria.");
            }
            return Ok(result);
        }


    }
}
