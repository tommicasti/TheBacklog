using GameShelf.API.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GameShelf.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        
        private readonly IGamesService _gamesService;

        public GamesController(IGamesService gamesService) 
        {
            _gamesService = gamesService;

        }

        // Esempio: GET /api/games/search?gameName=borderlands
        /// <summary>
        /// Searches for games based on the specified query string.
        /// </summary>
        /// <remarks>This method performs a case-insensitive search for games whose names match the
        /// provided query string. If no games match the query, an empty collection is returned.</remarks>
        /// <param name="query">The search term used to filter games. This parameter cannot be null or empty.</param>
        /// <returns>An <see cref="IActionResult"/> containing a collection of games that match the search query. The response is
        /// returned as an HTTP 200 OK status with the matching games, or an appropriate error status if the request
        /// fails.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchGames([FromQuery] string query)
        {
            var games = await _gamesService.SearchGamesAsync(query);
            return Ok(games);
        }

        // Endpoint per ottenere i dettagli di un singolo gioco
        // Esempio: GET /api/games/41494
        /// <summary>
        /// Retrieves the details of a specific game by its unique identifier.
        /// </summary>
        /// <remarks>This method is an HTTP GET endpoint that retrieves game details based on the provided
        /// <paramref name="id"/>. Ensure the <paramref name="id"/> corresponds to a valid game in the system.</remarks>
        /// <param name="id">The unique identifier of the game to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the game details if found;  otherwise, a <see
        /// cref="NotFoundResult"/> if the game does not exist.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameDetails(int id)
        {
            var game = await _gamesService.GetGameDetailsAsync(id);
            if (game == null) return NotFound();
            return Ok(game);
        }

        //Esempio: GET /api/games/popular
       /// <summary>
       /// Retrieves a list of popular games.
       /// </summary>
       /// <remarks>This method returns a collection of games that are currently popular, as determined by
       /// the underlying service.  The results are fetched asynchronously and returned in the HTTP response
       /// body.</remarks>
       /// <returns>An <see cref="IActionResult"/> containing the list of popular games. The response has a status code of 200
       /// (OK)  if the operation is successful, and the games are included in the response body.</returns>
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularGames()
        {
            var games = await _gamesService.GetPopularGamesAsync();
            return Ok(games);
        }



    }


}
