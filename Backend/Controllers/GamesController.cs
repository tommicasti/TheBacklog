using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GameShelf.API.Controllers
{
    [ApiController]
    [Route("api/games")]
    public class GamesController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _config;
        private readonly string _apiKey;

        public GamesController(IHttpClientFactory clientFactory, IConfiguration config) 
        {
            _clientFactory = clientFactory;
            _config= config;
            _apiKey = _config["RAWG:ApiKey"];
        }

        // Esempio: GET /api/games/search?gameName=borderlands
        /// <summary>
        /// Searches for games based on the specified game name by querying an external API.
        /// </summary>
        /// <remarks>This method sends a GET request to an external game database API to retrieve game
        /// information based on the provided game name. The response is returned as raw JSON with a content type of
        /// "application/json".</remarks>
        /// <param name="gameName">The name of the game to search for. This parameter is required and cannot be null or empty.</param>
        /// <returns>An <see cref="IActionResult"/> containing the JSON response from the external API if the search is
        /// successful. Returns a <see cref="BadRequestObjectResult"/> if <paramref name="gameName"/> is null or empty.
        /// Returns a <see cref="StatusCodeResult"/> with status code 502 if there is an error communicating with the
        /// external API.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchGames([FromQuery] string gameName, int pageSize)
        {

            if (string.IsNullOrEmpty(gameName))
            {
                return BadRequest("Gioco inesistente");
            }

            var client = _clientFactory.CreateClient();
            var requestUrl = $"https://api.rawg.io/api/games?key={_apiKey}&search={gameName}&ordering=-metacritic&page_size={pageSize}";

            try
            {
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                // per restituire direttamente il JSON da RAWG
                return Content(jsonResponse, "application/json");

            }
            catch (HttpRequestException e)
            {
                return StatusCode(502, $"Errore nella chiamata all'API esterna: {e.Message}");
            }
        }


        // Endpoint per ottenere i dettagli di un singolo gioco
        // Esempio: GET /api/games/41494
        /// <summary>
        /// Retrieves the details of a specific game by its unique identifier.
        /// </summary>
        /// <remarks>This method sends a GET request to an external API to fetch game details. The
        /// response is returned as JSON content. Ensure that the provided <paramref name="id"/> corresponds to a valid
        /// game identifier.</remarks>
        /// <param name="id">The unique identifier of the game to retrieve details for.</param>
        /// <returns>An <see cref="IActionResult"/> containing the game details in JSON format if the request is successful.
        /// Returns a 502 status code with an error message if the external API call fails.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameDetails(int id)
        {
            var client = _clientFactory.CreateClient();
            var requestUrl = $"https://api.rawg.io/api/games/{id}?key={_apiKey}";

            try
            {
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                return Content(jsonResponse, "application/json");
            }
            catch (HttpRequestException e)
            {
                return StatusCode(502, $"Errore nella chiamata all'API esterna: {e.Message}");
            }
        }

        // Esempio: GET /api/games/popular
        /// <summary>
        /// Retrieves a list of popular games from the past year.
        /// </summary>
        /// <remarks>This method sends a request to an external API to fetch games that have been added
        /// most frequently  over the past year. The results are ordered by the number of additions in descending
        /// order.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the JSON response from the external API with the list of popular
        /// games. If the external API call fails, a 502 Bad Gateway status code is returned with an error message.</returns>
        [HttpGet("popular")]
        public async Task<IActionResult> GetPopularGames()
        {
            var client = _clientFactory.CreateClient();
            var lastYear = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
            var today = DateTime.Now.ToString("yyyy-MM-dd");

            var requestUrl = $"https://api.rawg.io/api/games?key={_apiKey}&dates={lastYear},{today}&ordering=-added";

            try
            {
                var response = await client.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return Content(jsonResponse, "application/json");
            }
            catch (HttpRequestException e)
            {
                return StatusCode(502, $"Errore nella chiamata all'API esterna: {e.Message}");
            }
        }



    }


}
