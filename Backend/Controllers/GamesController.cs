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

        public GamesController(IHttpClientFactory _clientFactory, IConfiguration config) 
        {
            _clientFactory = _clientFactory;
            _config= config;
            _apiKey = _config["RAWG:ApiKey"];
        }

        // Esempio: GET /api/games/search?gameName=borderlands
        [HttpGet("search")]
        public async Task<IActionResult> SearchGames([FromQuery] string gameName)
        {

            if (string.IsNullOrEmpty(gameName))
            {
                return BadRequest("Gioco inesistente");
            }

            var client = _clientFactory.CreateClient();
            var requestUrl = $"https://api.rawg.io/api/games?key={_apiKey}&search={gameName}";

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



    }


}
